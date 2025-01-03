using FeedService.Domain.DTOs.External.DataFeedWatch;
using FeedService.Domain.Models;
using FeedService.Services.SalesAreaConfigurationServices;
using Microsoft.Extensions.Options;
using SharedLib.Clients.Norce;
using SharedLib.Logging.Enums;
using SharedLib.Options.Models;

namespace FeedService.Services.CultureFeedGenerationServices
{
    /// <summary>
    /// Service responsible for fetching and generating feed data with culture-specific attributes.
    /// Handles all external API calls and coordinates the data transformation process.
    /// </summary>
    public class CultureFeedGenerationService(
        ILogger<CultureFeedGenerationService> logger,
        INorceClient norceClient,
        IOptionsMonitor<NorceBaseModuleOptions> norceOptions,
        IOptionsMonitor<NorceProductFeedModuleOptions> norceProductFeedOptions,
        IOptionsMonitor<BaseModuleOptions> baseOptions)
        : ICultureFeedGenerationService
    {
        /// <summary>
        /// Fetches product data from Norce and generates a feed with culture-specific attributes.
        /// </summary>
        /// <param name="cultures">List of culture configurations to process</param>
        /// <param name="traceId">Unique identifier for request tracing</param>
        /// <returns>A list of processed products with culture-specific attributes</returns>
        /// <exception cref="ArgumentNullException">Thrown when cultures parameter is null</exception>
        /// <exception cref="InvalidOperationException">Thrown when unable to process the feed</exception>
        public async Task<List<CultureConfiguration>> GenerateFeedWithCultures(List<CultureConfiguration> cultures, Guid? traceId = null)
        {
            try
            {
                logger.LogInformation(
                    "TraceId: {traceId} Service: {serviceName} LogType: {logType} Method: {method} Message: {message} | Other Parameters Action: {action}",
                    traceId,
                    nameof(CultureFeedGenerationService),
                    nameof(LoggingTypes.CheckpointLog),
                    nameof(GenerateFeedWithCultures),
                    "Starting feed generation with cultures",
                    nameof(MethodActionLogTypes.Starting)
                );

                var processedProducts = new List<DataFeedWatchDto>();

                // Fetch all products from Norce
                await foreach (var product in norceClient.ProductFeed.StreamProductFeedAsync(norceProductFeedOptions.CurrentValue.ChannelKey, cancellationToken: default))
                {
                    if (product != null)
                    {
                        // Process each product for each culture
                        foreach (var culture in cultures)
                        {
                            logger.LogInformation(
                                "TraceId: {traceId} Service: {serviceName} LogType: {logType} Method: {method} Message: {message} | Other Parameters ProductCode: {productCode}, Culture: {culture}",
                                traceId,
                                nameof(CultureFeedGenerationService),
                                nameof(LoggingTypes.CheckpointLog),
                                nameof(GenerateFeedWithCultures),
                                "Processing product for culture",
                                product.Code,
                                culture.CultureCode
                            );

                            var processedProduct = CultureFeedGenerationServiceExtension.MapProductForCulture(
                                product,
                                culture,
                                norceOptions,
                                baseOptions,
                                logger,
                                traceId);

                            if (processedProduct != null)
                            {
                                culture.Products.AddRange(processedProduct);
                            }
                        }
                    }
                }

                logger.LogInformation(
                    "TraceId: {traceId} Service: {serviceName} LogType: {logType} Method: {method} Message: {message} | Other Parameters Action: {action}, ProductCount: {count}",
                    traceId,
                    nameof(CultureFeedGenerationService),
                    nameof(LoggingTypes.CheckpointLog),
                    nameof(GenerateFeedWithCultures),
                    "Completed feed generation with cultures",
                    nameof(MethodActionLogTypes.Completed),
                    processedProducts.Count
                );

                return cultures;
            }
            catch (Exception ex)
            {
                logger.LogError(
                    ex,
                    "TraceId: {traceId} Service: {serviceName} LogType: {logType} Method: {method} Error Source: {errorSource} Error Message: {errorMessage} Error Stacktrace: {errorStackTrace} Error Inner Exception: {errorInnerException} Internal Message: {internalMessage} | Other Parameters CultureCount: {cultureCount}",
                    traceId,
                    nameof(CultureFeedGenerationService),
                    nameof(LoggingTypes.ErrorLog),
                    nameof(GenerateFeedWithCultures),
                    ex.Source,
                    ex.Message,
                    ex.StackTrace,
                    ex.InnerException,
                    "Failed to generate culture feeds",
                    cultures?.Count ?? 0
                );
                throw;
            }
        }
    }
}
