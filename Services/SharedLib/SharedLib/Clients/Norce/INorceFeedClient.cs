using FeedService.Domain.Norce;

namespace SharedLib.Clients.Norce;

public interface INorceFeedClient
{
    /// <summary>
    /// Trigger a product export from Norce. If deltaFromDate parameter is set, the method will fetch a delta feed from specified date
    /// </summary>
    /// <param name="channelKey">The Norce channel key</param>
    /// <param name="deltaFromDate">Used to fetch from specific date. If not set, method fetches a full feed</param>
    /// <param name="cancellationToken">Optional cancellation token. Defaults to <see cref="CancellationToken.None"/></param>
    /// <returns></returns>
    Task<Stream> FetchProductExport(string channelKey, DateTime? deltaFromDate, CancellationToken? cancellationToken);

    /// <summary>
    /// Stream products from a Norce channel feed
    /// </summary>
    /// <param name="exportId"></param>
    /// <param name="deltaFromDate"></param>
    /// <param name="cancellationToken">Optional cancellation token. Defaults to <see cref="CancellationToken.None"/></param>
    /// <returns></returns>
    IAsyncEnumerable<NorceFeedProductDto?> StreamProductFeedAsync(string exportId, DateTime? deltaFromDate = null, CancellationToken cancellationToken = default);
}
