using FeedService.Domain.Models;

namespace FeedService.Services.SalesAreaConfigurationServices
{
    /// <summary>
    /// Service for managing and retrieving sales area configurations from Norce.
    /// </summary>
    public interface ISalesAreaConfigurationService
    {
        /// <summary>
        /// Retrieves a list of sales area configurations with their associated currency and pricelist information.
        /// </summary>
        /// <param name="traceId">Unique identifier for request tracing</param>
        /// <returns>A list of SalesAreaConfiguration objects containing sales area settings</returns>
        /// <exception cref="InvalidOperationException">Thrown when unable to retrieve or parse configuration data</exception>
        /// <exception cref="ArgumentNullException">Thrown when required data is missing from the Norce API response</exception>
        Task<List<SalesAreaConfiguration>> GetSalesAreaConfigurations(Guid? traceId = null);
    }
}