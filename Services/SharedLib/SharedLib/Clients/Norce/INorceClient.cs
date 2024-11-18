namespace SharedLib.Clients.Norce;

/// <summary>
/// Base client for Norce service clients. Access the specific service client methods by its extended client. <br />
/// Example usage: <br />
/// <code>
/// var products = await NorceClient.Connect.ImportProducts();
/// </code>
/// </summary>
public interface INorceClient
{
    /// <summary>
    /// Performs a GET request to the Norce service.
    /// </summary>
    /// <param name="query">The query string for the GET request. This should include any necessary parameters.</param>
    /// <param name="applicationId">Optional application ID for the request. Defaults to null.</param>
    /// <returns>A task representing the asynchronous operation, with a string containing the response data.</returns>
    Task<string> BaseGetAsync(string query, string? applicationId = null);

    /// <summary>
    /// Performs a POST request to the Norce service.
    /// </summary>
    /// <param name="query">The query string for the POST request. This should include any necessary parameters.</param>
    /// <param name="json">The JSON payload to be sent in the POST request body.</param>
    /// <param name="applicationId">Optional application ID for the request. Defaults to null.</param>
    /// <returns>A task representing the asynchronous operation, with an <see cref="HttpResponseMessage"/> containing the response.</returns>
    Task<HttpResponseMessage> BasePostAsync(string query, string json, string? applicationId = null);

    /// <summary>
    /// Performs a POST request with custom headers to the Norce service.
    /// </summary>
    /// <typeparam name="THeader">The type of the custom header object.</typeparam>
    /// <param name="query">The query string for the POST request. This should include any necessary parameters.</param>
    /// <param name="json">The JSON payload to be sent in the POST request body.</param>
    /// <param name="header">The custom header object to be included in the request.</param>
    /// <param name="applicationId">Optional application ID for the request. Defaults to null.</param>
    /// <returns>A task representing the asynchronous operation, with an <see cref="HttpResponseMessage"/> containing the response.</returns>
    Task<HttpResponseMessage> BasePostAsync<THeader>(string query, string json, THeader header, string? applicationId = null);

    /// <summary>
    /// Gets the ID of the Norce system user.
    /// </summary>
    int UserId { get; }

    /// <summary>
    /// Gets the client for the Norce Connect API.
    /// </summary>
   INorceConnectClient Connect { get; }

    /// <summary>
    /// Gets the client for the Norce API.
    /// </summary>
    NorceApiClient Api { get; }

    /// <summary>
    /// Gets the client for the Norce Query API.
    /// </summary>
    NorceQueryClient Query { get; }

    /// <summary>
    /// Gets the client for the Norce Feed API.
    /// </summary>
    INorceFeedClient ProductFeed { get; }

}
