using System.Text.Json;
using FeedService.Domain.Models;
using FeedService.Services.CultureFeedGenerationServices;
using Microsoft.Extensions.Options;
using SharedLib.Clients.Norce;
using SharedLib.Logging.Enums;
using SharedLib.Models.Norce.Api;
using SharedLib.Options.Models;

namespace FeedService.Services.PriceFeedGenerationServices
{
    public class PriceFeedGenerationService(
        ILogger<PriceFeedGenerationService> _logger,
        INorceClient _norceClient,
        IOptionsMonitor<NorceBaseModuleOptions> _norceOptions,
        IOptionsMonitor<BaseModuleOptions> _baseOptions) : IPriceFeedGenerationService
    {
        /// <summary>
        /// Generates a feed with price information for each sales area.
        /// </summary>
        /// <param name="salesAreas">List of sales area configurations to process</param>
        /// <param name="traceId">Unique identifier for request tracing</param>
        /// <returns>List of sales area configurations with populated price information</returns>
        /// <exception cref="ArgumentNullException">Thrown when salesAreas is null</exception>
        /// <exception cref="InvalidOperationException">Thrown when unable to process price feed</exception>
        public async Task<List<SalesAreaConfiguration>> GenerateFeedWithPrices(List<SalesAreaConfiguration> salesAreas, Guid? traceId = null)
        {
            try
            {
                if (salesAreas == null) throw new ArgumentNullException(nameof(salesAreas));

                _logger.LogInformation(
                    "TraceId: {traceId} Service: {serviceName} LogType: {logType} Method: {method} Message: {message} | Other Parameter Action: {action}",
                    traceId,
                    nameof(PriceFeedGenerationService),
                    nameof(LoggingTypes.CheckpointLog),
                    nameof(GenerateFeedWithPrices),
                    "Starting price feed generation",
                    nameof(MethodActionLogTypes.Starting)
                );

                foreach (var salesArea in salesAreas)
                {
                    try
                    {
                        var statusSeeds = new List<int> { 1 };
                        var products = await _norceClient.Api.GetProducts(statusSeeds, salesArea.PriceListIds, salesArea.SalesAreaId);
                        if (products == null || !products.Any())
                        {
                            _logger.LogWarning(
                                "TraceId: {traceId} Service: {serviceName} LogType: {logType} Method: {method} Message: {message} | Other Parameters SalesAreaId: {salesAreaId}",
                                traceId,
                                nameof(PriceFeedGenerationService),
                                nameof(LoggingTypes.IssueLog),
                                nameof(GenerateFeedWithPrices),
                                "No price data found for sales area",
                                salesArea.SalesAreaId
                            );
                            continue;
                        }

                        salesArea.ProductsPriceInfo = PriceFeedGenerationServiceExtension.MapListProductsToPriceProducts(products, _logger, traceId);

                        _logger.LogInformation(
                            "TraceId: {traceId} Service: {serviceName} LogType: {logType} Method: {method} Message: {message} | Other Parameters SalesAreaId: {salesAreaId}, ProductCount: {productCount}, ValidProducts: {validProducts}, InvalidProducts: {invalidProducts}",
                            traceId,
                            nameof(PriceFeedGenerationService),
                            nameof(LoggingTypes.CheckpointLog),
                            nameof(GenerateFeedWithPrices),
                            "Successfully processed price data for sales area",
                            salesArea.SalesAreaId,
                            products.Count,
                            salesArea.ProductsPriceInfo.Count,
                            products.Count- salesArea.ProductsPriceInfo.Count
                        );
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(
                            ex,
                            "TraceId: {traceId} Service: {serviceName} LogType: {logType} Method: {method} Error Source: {errorSource} Error Message: {errorMessage} Error Stacktrace: {errorStackTrace} Error Inner Exception: {errorInnerException} Internal Message: {internalMessage} | Other Parameters SalesAreaId: {salesAreaId}",
                            traceId,
                            nameof(PriceFeedGenerationService),
                            nameof(LoggingTypes.ErrorLog),
                            nameof(GenerateFeedWithPrices),
                            ex.Source,
                            ex.Message,
                            ex.StackTrace,
                            ex.InnerException,
                            "Failed to process price data for sales area",
                            salesArea.SalesAreaId
                        );
                        // Continue processing other sales areas even if one fails
                    }
                }

                _logger.LogInformation(
                    "TraceId: {traceId} Service: {serviceName} LogType: {logType} Method: {method} Message: {message} | Other Parameter Action: {action}",
                    traceId,
                    nameof(PriceFeedGenerationService),
                    nameof(LoggingTypes.CheckpointLog),
                    nameof(GenerateFeedWithPrices),
                    "Completed price feed generation",
                    nameof(MethodActionLogTypes.Completed)
                );

                return salesAreas;
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "TraceId: {traceId} Service: {serviceName} LogType: {logType} Method: {method} Error Source: {errorSource} Error Message: {errorMessage} Error Stacktrace: {errorStackTrace} Error Inner Exception: {errorInnerException} Internal Message: {internalMessage}",
                    traceId,
                    nameof(PriceFeedGenerationService),
                    nameof(LoggingTypes.ErrorLog),
                    nameof(GenerateFeedWithPrices),
                    ex.Source,
                    ex.Message,
                    ex.StackTrace,
                    ex.InnerException,
                    "Failed to generate price feed"
                );
                throw;
            }
        }
    }

}
