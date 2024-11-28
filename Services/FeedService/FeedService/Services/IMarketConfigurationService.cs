using FeedService.Domain.Models;

namespace FeedService.Services
{
    public interface IMarketConfigurationService
    {
        /// <summary>
        /// Gets all available markets and their configurations
        /// </summary>
        Task<List<MarketConfiguration>> GetMarketConfigurations(Guid? traceId = null);

       
    }
}
