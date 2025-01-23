using FeedService.Domain.DTOs.External.DataFeedWatch;
using FeedService.Domain.Models;

namespace FeedService.Services.CultureFeedGenerationServices
{
    public interface ICultureFeedGenerationService
    {
        Task<List<CultureConfiguration>> GenerateFeedWithCultures(List<CultureConfiguration> cultures, CancellationToken cancellationToken, Guid? traceId = null);
    }
}