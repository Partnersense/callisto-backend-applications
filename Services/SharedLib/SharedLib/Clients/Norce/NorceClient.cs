using System.Text;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Polly;
using SharedLib.Constants;
using SharedLib.Options.Models;
using SharedLib.RetryPolicy;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace SharedLib.Clients.Norce;

public class NorceClient : INorceClient
{
    private readonly ILogger<NorceClient> _logger;
    private readonly HttpClient _httpClient;
    private readonly IAsyncPolicy<HttpResponseMessage> _norceRetryPolicy;
    private readonly IAsyncPolicy<HttpResponseMessage> _basicPolicy;
    private readonly NorceBaseModuleOptions _config;

    public int UserId { get; }
    public INorceConnectClient Connect { get; }
    public NorceApiClient Api { get; }
    public NorceQueryClient Query { get; }
    public INorceFeedClient ProductFeed { get; }

    public NorceClient(ILogger<NorceClient> logger, IHttpClientFactory httpClientFactory, INorceRetryPolicy norceRetry, IOptionsMonitor<NorceBaseModuleOptions> norceConfig, INorceConnectClient? connectClient = null, INorceFeedClient? productFeedClient = null)
    {
        _logger = logger;
        _httpClient = httpClientFactory.CreateClient(nameof(NorceClient));
        _config = norceConfig.CurrentValue;
        _httpClient.BaseAddress = new Uri(_config.BaseUrl);
        var token = GetAccessTokenAsync().Result;
        _httpClient.DefaultRequestHeaders.Add(HttpConstants.Headers.Authorization, $"Bearer {token}");
        _norceRetryPolicy = norceRetry.InitiateNorceHttpRetryPolicy(RequestNewToken);
        _basicPolicy = norceRetry.BasicPolicy;

        UserId = _config.UserId;
        Connect = connectClient ?? new NorceConnectClient(this, _logger);
        Api = new NorceApiClient(this);
        Query = new NorceQueryClient(this);
        ProductFeed = productFeedClient ?? new NorceFeedClient(this, _logger);
    }

    public async Task<string> GetAccessTokenAsync()
    {
        var requestBody = $"client_id={_config.ClientId}" +
                          $"&client_secret={_config.ClientSecret}" +
                          $"&grant_type=client_credentials" +
                          $"&scope={_config.Environment}";

        var tokenRequest = new HttpRequestMessage
        {
            RequestUri = new Uri($"{_config.BaseUrl}{Endpoints.Auth}"),
            Content = new StringContent(requestBody,
                Encoding.UTF8,
                "application/x-www-form-urlencoded"),
            Method = HttpMethod.Post
        };

        var response = await _httpClient.SendAsync(tokenRequest);

        var returnValue = JsonSerializer.Deserialize<Dictionary<string, object>>(response.Content.ReadAsStringAsync().GetAwaiter().GetResult());
        return returnValue?["access_token"].ToString() ?? "";
    }

    /// <summary>
    /// Get
    /// </summary>
    /// <param name="query"></param>
    /// <param name="applicationId"></param>
    /// <returns>string content</returns>
    public async Task<string> BaseGetAsync(string query, string? applicationId = null)
    {
        var response = await _basicPolicy.WrapAsync(_norceRetryPolicy).ExecuteAsync(async () =>
        {
            var request = new HttpRequestMessage(HttpMethod.Get, query);
            request.Headers.Add(HttpConstants.Headers.ApplicationId, applicationId ?? _config.ApplicationId);
            return await _httpClient.SendAsync(request);
        });
        _logger.LogDebug("Response received with status: {ResponseStatus} and message: {ResponseMessage}", response.StatusCode, response.ReasonPhrase);
        return await response.Content.ReadAsStringAsync();
    }

    /// <summary>
    /// Post
    /// </summary>
    /// <param name="query"></param>
    /// <param name="json"></param>
    /// <param name="applicationId"></param>
    /// <returns>string content</returns>
    public async Task<HttpResponseMessage> BasePostAsync(string query, string json, string? applicationId = null)
    {
        var content = new StringContent(json, Encoding.UTF8, HttpConstants.Content.Json);
        var response = await _basicPolicy.WrapAsync(_norceRetryPolicy).ExecuteAsync(async () =>
        {
            var request = new HttpRequestMessage(HttpMethod.Post, query);
            request.Content = content;
            request.Headers.Add(HttpConstants.Headers.ApplicationId, applicationId ?? _config.ApplicationId);
            return await _httpClient.SendAsync(request);
        });
        _logger.LogDebug("Response received with status: {ResponseStatus} and message: {ResponseMessage}", response.StatusCode, response.ReasonPhrase);
        return response;
    }

    /// <summary>
    /// Post for connect client using StormConnect-Header
    /// </summary>
    /// <param name="query"></param>
    /// <param name="json"></param>
    /// <param name="header"></param>
    /// <param name="applicationId"></param>
    /// <returns>string content</returns>
    public async Task<HttpResponseMessage> BasePostAsync<THeader>(string query, string json, THeader header, string? applicationId = null)
    {
        var content = new StringContent(json, Encoding.UTF8, HttpConstants.Content.Json);
        var response = await _basicPolicy.WrapAsync(_norceRetryPolicy).ExecuteAsync(async () =>
        {
            var request = new HttpRequestMessage(HttpMethod.Post, query);
            request.Content = content;
            request.Headers.Add(HttpConstants.Headers.ApplicationId, applicationId ?? _config.ApplicationId);
            request.Headers.Add(HttpConstants.Headers.StormConnect, JsonSerializer.Serialize(header));
            return await _httpClient.SendAsync(request);
        });
        _logger.LogDebug("Response received with status: {ResponseStatus} and message: {ResponseMessage}", response.StatusCode, response.ReasonPhrase);
        return response;
    }

    private void RequestNewToken()
    {
        var token = GetAccessTokenAsync().Result;
        _httpClient.DefaultRequestHeaders.Remove(HttpConstants.Headers.Authorization);
        _httpClient.DefaultRequestHeaders.Add(HttpConstants.Headers.Authorization, $"Bearer {token}");
    }
}
