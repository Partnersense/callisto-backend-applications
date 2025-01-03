using FeedService.Domain.DTOs.External.DataFeedWatch;
using FeedService.Domain.Models;

namespace FeedService.Services.CultureFeedGenerationServices
{
    public interface ICultureFeedGenerationService
    {
        Task<List<DataFeedWatchDto>> GenerateFeedWithCultures(List<CultureConfiguration> cultures, Guid? traceId = null);
    }
}