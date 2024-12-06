namespace FeedService.Services.PriceFeedGenerationServices;

public interface IPriceFeedGenerationService
{
    Task<List<object>> GenerateFeedWithPrices(object cultureFeed, Guid? traceId = null);
}