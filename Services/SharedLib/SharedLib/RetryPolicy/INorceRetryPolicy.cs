using Polly;
using Polly.Retry;
using SharedLib.Clients.Norce;

namespace SharedLib.RetryPolicy;

/// <summary>
/// Retry policy specific to Norce clients as it will also handle <see cref="HttpRequestException"/> with status code 401
/// (unauthorized). Has a dependency for a <see cref="IRetryDelayCalculator"/>.
/// </summary>
public interface INorceRetryPolicy
{
    /// <summary>
    /// Initiates <see cref="NorceRetryPolicy"/> using the method passed in <paramref name="tokenRefresh"/>
    /// to set an onRetry handler for <see cref="HttpRequestException"/> with status code 401, in which case it will call
    /// the code passed in <paramref name="tokenRefresh"/> to refresh the access token if it has expired.
    /// </summary>
    /// <param name="tokenRefresh">Method sent from <see cref="NorceClient"/> used to refresh an access token</param>
    /// <returns><see cref="AsyncRetryPolicy"/> specific to Norce clients</returns>
    IAsyncPolicy<HttpResponseMessage> InitiateNorceHttpRetryPolicy(Action tokenRefresh);

    IAsyncPolicy<HttpResponseMessage> BasicPolicy { get; }
}