using FeedService.Domain.Models;

namespace FeedService.Services.CultureFeedGenerationServices
{
    public interface ICultureFeedGenerationService
    {
        Task<List<MarketFeed>> GenerateFeedWithCultures(List<CultureConfiguration> cultures, Guid? traceId = null);
    }
}