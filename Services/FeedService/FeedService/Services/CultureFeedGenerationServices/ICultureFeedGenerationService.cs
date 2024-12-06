using FeedService.Domain.Models;

namespace FeedService.Services.CultureFeedGenerationServices
{
    public interface ICultureFeedGenerationService
    {
        Task<List<object>> GenerateFeedWithCultures(List<CultureConfiguration> cultures, Guid? traceId = null);
    }
}