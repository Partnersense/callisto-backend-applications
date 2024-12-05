using FeedService.Domain.Models;

namespace FeedService.Services.CultureConfigurationServices
{
    public interface ICultureConfigurationService
    {
        /// <summary>
        /// Gets all available markets and their configurations
        /// </summary>
        Task<List<CultureConfiguration>> GetCultureConfigurations(Guid? traceId = null);


    }
}
