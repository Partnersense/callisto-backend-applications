using FeedService.Domain.Models;

namespace FeedService.Services.PriceFeedGenerationServices;

public interface IPriceFeedGenerationService
{
    Task<List<object>> GenerateFeedWithPrices(List<SalesAreaConfiguration> salesAreas, Guid? traceId = null);
}