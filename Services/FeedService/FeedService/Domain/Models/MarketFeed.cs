using FeedService.Domain.DTOs.External.DataFeedWatch;

namespace FeedService.Domain.Models;

public class MarketFeed
{
    public string Market { get; set; } = string.Empty;
    public List<DataFeedWatchDto> Products { get; set; } = [];
}
