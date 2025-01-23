using FeedService.Domain.Models;

namespace FeedService.Services.PriceFeedGenerationServices;

public interface IPriceFeedGenerationService
{
    Task<List<SalesAreaConfiguration>> GenerateFeedWithPrices(List<SalesAreaConfiguration> salesAreas, Guid? traceId = null);
}