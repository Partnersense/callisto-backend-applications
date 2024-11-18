using System.Text.Json;
using Enferno.Services.StormConnect.Contracts.Product;
using Enferno.Services.StormConnect.Contracts.Product.Fields;
using Enferno.Services.StormConnect.Contracts.Product.Models;
using Microsoft.Extensions.Logging;
using MoreLinq;

namespace SharedLib.Clients.Norce;

public class NorceConnectClient(INorceClient client, ILogger<NorceClient> logger) : INorceConnectClient
{
    
    private const bool OnlyImportOneBatchDev = false;

    /// <summary>
    /// Import data with headers using Norce Connect API
    /// </summary>
    /// <param name="serviceEndpoint">query url</param>
    /// <param name="json"></param>
    /// <param name="header"></param>
    /// <param name="applicationId"></param>
    /// <returns></returns>
    public async Task<HttpResponseMessage> PostAsync<THeader>(string serviceEndpoint, string json, THeader header, string? applicationId = null)
    {
        return await client.BasePostAsync(Endpoints.GetEndpoint(Endpoints.Connect.Base, serviceEndpoint), json, header, applicationId);
    }

    /// <summary>
    /// Import data without headers using Norce Connect API
    /// </summary>
    /// <param name="serviceEndpoint">query url</param>
    /// <param name="json"></param>
    /// <param name="applicationId"></param>
    /// <returns></returns>
    public async Task<HttpResponseMessage> PostAsync(string serviceEndpoint, string json, string? applicationId = null)
    {
        return await client.BasePostAsync(Endpoints.GetEndpoint(Endpoints.Connect.Base, serviceEndpoint), json, applicationId);
    }

    /// <summary>
    /// Get data from Norce Connect API
    /// </summary>
    /// <param name="serviceEndpoint">query url</param>
    /// <param name="applicationId"></param>
    /// <returns></returns>
    public async Task<string> GetAsync(string serviceEndpoint, string? applicationId = null)
    {
        return await client.BaseGetAsync(Endpoints.GetEndpoint(Endpoints.Connect.Base, serviceEndpoint), applicationId);
    }

    /// <summary>
    /// Health check ping from Norce Connect API
    /// </summary>
    /// <param name="applicationId"></param>
    /// <returns></returns>
    public async Task<string> Ping(string? applicationId = null)
    {
        return await client.BaseGetAsync(Endpoints.Connect.Job.Ping, applicationId);
    }

    /// <summary>
    /// Import products
    /// </summary>
    /// <param name="products"></param>
    /// <param name="header"></param>
    /// <param name="batchSize"></param>
    /// <param name="applicationId"></param>
    /// <returns></returns>
    public async Task<bool> ImportProducts(IEnumerable<Product> products, ProductHeader header, int batchSize, string? applicationId = null)
    {
        return await BatchImport(products, Endpoints.Connect.Product.ImportProducts, header, batchSize, applicationId);
    }

    /// <summary>
    /// Import OnHands
    /// </summary>
    /// <param name="batchSize">Size of the import batch</param>
    /// <param name="applicationId">The Norce ApplicationId. Defaults to null</param>
    /// <param name="onHands">The List of <see cref="SkuOnhand"/> to be imported</param>
    /// <returns></returns>
    public async Task<bool> ImportOnHands(IEnumerable<SkuOnhand> onHands, int batchSize, string? applicationId = null)
    {
        var header = new SkuOnhandHeader
        {
            FullFile = false,
            AccountId = client.UserId,
            SkuOnhandFieldsThatAreSet = new List<SkuOnhandField>()
            {
                SkuOnhandField.OnhandValue,
            }
        };

        return await BatchImport(onHands, Endpoints.Connect.Product.ImportOnhands, header, batchSize, applicationId);
    }

    /// <summary>
    /// Import SkuPriceLists
    /// </summary>
    /// <param name="batchSize"></param>
    /// <param name="applicationId"></param>
    /// <param name="prices"></param>
    /// <returns></returns>
    public async Task<bool> ImportSkuPriceLists(IEnumerable<SkuPriceList> prices, int batchSize, string? applicationId = null)
    {
        var header = new SkuPriceListHeader
        {
            FullFile = false,
            AccountId = client.UserId,
            SkuPriceListFieldsThatAreSet = new List<SkuPriceListField>()
            {
                SkuPriceListField.PriceSale
            }
        };

        return await BatchImport(prices, Endpoints.Connect.Product.ImportSkuPriceLists, header, batchSize, applicationId);

    }

    private async Task<bool> BatchImport<T, THeader>(IEnumerable<T> importList, string endpoint, THeader header, int batchSize, string? applicationId = null)
    {
        var success = true;
        var count = 1;
        var enumerable = importList as T[] ?? importList.ToArray();
        var totalBatches = Math.Ceiling((double)enumerable.Length / batchSize);
        foreach (var batch in enumerable.Batch(batchSize))
        {
            if (count > 1 && OnlyImportOneBatchDev)
            {
                return success;
            }
            var json = JsonSerializer.Serialize(batch);
            logger.LogInformation("Trying to import batch {Batch}/{Total} of {Type} to Norce", count, totalBatches, typeof(T));
            var result = await client.BasePostAsync(endpoint, json, header, applicationId);
            count++;
            if (result.IsSuccessStatusCode) continue;
            logger.LogError("Something went wrong when importing batch {Batch}/{Total} of {Type} to Norce. Continuing importing remaining items.", count, totalBatches, nameof(T));
            success = false;
        }

        return success;
    }
}