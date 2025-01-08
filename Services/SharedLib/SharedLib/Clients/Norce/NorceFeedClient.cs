using System.Text.Json;
using Microsoft.Extensions.Logging;
using static SharedLib.Constants.NorceConstants.NorceConstants;
using SharedLib.Models.Norce;
using FeedService.Domain.Norce;
using System.Runtime.CompilerServices;
using Log = Serilog.Log;

namespace SharedLib.Clients.Norce;
public class NorceFeedClient(INorceClient client, ILogger<NorceClient> logger) : INorceFeedClient
{
    private static readonly HttpClient HttpClient = new();

    public const int MaxRetries = 30;
    public const int InitialDelaySeconds = 4000; // 4 seconds
    public const int MaxDelaySeconds = 600000; // 10 minutes

    public async IAsyncEnumerable<NorceFeedProductDto?> StreamProductFeedAsync(string exportId, DateTime? deltaFromDate = null, [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        await using var productFeedStream = await FetchProductExport(exportId, deltaFromDate, cancellationToken);
        using var file = new StreamReader(productFeedStream);

        while (await file.ReadLineAsync(cancellationToken) is { } line)
        {
            List<NorceFeedProductDto>? products;
            try
            {
                products = JsonSerializer.Deserialize<List<NorceFeedProductDto>>(line);
                if (products is null)
                {
                    Log.Logger.Warning("Could not map product properly. {@Line}", line);
                    continue;
                }
            }
            catch (JsonException ex)
            {
                Log.Logger.Warning(ex, "Could not deserialize feed product {@Line}.", line);
                continue;
            }

            foreach (var norceFeedProductDto in products) yield return norceFeedProductDto;
        }
    }

    public async Task<ProductExportResponse?> TriggerProductExport(ProductExportRequest request)
    {
        logger.LogInformation("Trying to trigger product export");

        var body = JsonSerializer.Serialize(request);
        var response = await client.BasePostAsync("/commerce/productfeed/1.0/api/v1/exports/product", body);
        if (!response.IsSuccessStatusCode)
        {
            logger.LogError("Could not trigger product export. StatusCode {StatusCode}", response.StatusCode);
            return null;
        }

        var json = await response.Content.ReadAsStringAsync();

        try
        {
            var productExport = JsonSerializer.Deserialize<ProductExportResponse>(json);
            return productExport;
        }
        catch (JsonException ex)
        {
            logger.LogError(ex, "Could not deserialize product export correctly");
        }

        return null;

    }

    public async Task<ProductExportJobResponse?> GetJob(string jobKey)
    {
        logger.LogInformation("Trying to fetch export job with key {Key}", jobKey);
        var response = await client.BaseGetAsync($"/commerce/productfeed/1.0/api/v1/jobs/job?jobKey={jobKey}");

        if (string.IsNullOrEmpty(response))
        {
            logger.LogInformation("Could not fetch export job with key {Key}", jobKey);
            return null;
        }

        try
        {
            var productExport = JsonSerializer.Deserialize<ProductExportJobResponse>(response);
            return productExport;
        }
        catch (JsonException ex)
        {
            logger.LogError(ex, "Could not deserialize product export correctly");
        }

        return null;
    }

    public async Task<Stream> FetchProductExport(string channelKey, DateTime? deltaFromDate, CancellationToken? cancellationToken)
    {
        var stoppingToken = cancellationToken ?? CancellationToken.None;
        string? delta = null;
        if (deltaFromDate.HasValue)
        {
            delta = deltaFromDate.Value.ToString("yyyy-MM-ddTHH:mm:ss.fffZ");
        }

        var request = new ProductExportRequest
        {
            ChannelKey = channelKey,
            DeltaFromDate = delta
        };

        var response = await TriggerProductExport(request);
        if (response is null || string.IsNullOrEmpty(response.JobKey)) return new MemoryStream();

        var retries = 0;
        var delay = InitialDelaySeconds;

        while (retries <= MaxRetries)
        {
            var job = await GetJob(response.JobKey);
            if (job?.StatusId == Export.Status.CompletedInExportFeed)
            {
                var result = await HttpClient.GetAsync(response.DataUrl, stoppingToken);
                var feed = await result.Content.ReadAsStreamAsync(stoppingToken);

                return feed;
            }

            logger.LogInformation("Job has not been completed yet. Waiting for {Delay} seconds before retrying again", delay / 1000);
            await Task.Delay(delay, stoppingToken);
            delay = Math.Min(delay * 2, MaxDelaySeconds); 
            retries++;
        }

        logger.LogError("Could not fetch product feed after {Retries}.", retries);
        return new MemoryStream(); 
    }
}
