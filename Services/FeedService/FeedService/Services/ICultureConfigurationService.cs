using FeedService.Domain.Models;

namespace FeedService.Services
{
    public interface ICultureConfigurationService
    {
        /// <summary>
        /// Gets all available markets and their configurations
        /// </summary>
        Task<List<MarketConfiguration>> GetCultureConfigurations(Guid? traceId = null);

       
    }
}
