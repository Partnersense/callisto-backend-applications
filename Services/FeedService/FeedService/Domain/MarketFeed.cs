using FeedService.Domain.DataFeedWatch;

namespace FeedService.Domain;

public class MarketFeed
{
    public string Market { get; set; } = string.Empty;
    public List<GenericFeedProductDto> Products { get; set; } = [];
}
