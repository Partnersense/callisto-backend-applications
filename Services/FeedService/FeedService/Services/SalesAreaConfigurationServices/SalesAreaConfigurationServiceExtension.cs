using FeedService.Domain.Models;
using FeedService.Services.CultureConfigurationServices;
using SharedLib.Logging.Enums;
using SharedLib.Models.Norce.Query;

namespace FeedService.Services.SalesAreaConfigurationServices
{
    public static class SalesAreaConfigurationServiceExtension
    {
        /// <summary>
        /// Maps a SalesAreaResponse to a SalesAreaConfiguration object, including associated pricelists and currency information.
        /// </summary>
        /// <param name="salesArea">The sales area data from Norce API</param>
        /// <param name="allPriceLists">List of all available price lists</param>
        /// <param name="currencies">List of all available currencies</param>
        /// <param name="traceId">Unique identifier for request tracing</param>
        /// <returns>A SalesAreaConfiguration object if mapping is successful, null if the sales area has no associated pricelists</returns>
        /// <exception cref="ArgumentNullException">Thrown when required mapping data is missing</exception>
        /// <exception cref="InvalidOperationException">Thrown when unable to perform the mapping due to data inconsistencies</exception>
        /// <remarks>
        /// The method filters pricelists by sales area and only includes active pricelists.
        /// If no pricelists are found for the sales area, it returns null and logs a warning.
        /// </remarks>
        public static SalesAreaConfiguration? MapSalesAreaToSalesAreaConfiguration(SalesAreaResponse salesArea, List<PriceListClientResponse> allPriceLists, List<CurrenciesResponse> currencies, ILogger _logger, Guid? traceId = null)
        {
            try
            {
                // Get pricelists for this sales area
                var salesAreaPriceLists = allPriceLists
                    .Where(pl => pl.IsActive && (pl.SalesAreaId == salesArea.SalesAreaId || salesArea.IsPrimary && pl.SalesAreaId == null))
                    .ToList();

                if (salesAreaPriceLists == null || !salesAreaPriceLists.Any())
                {
                    _logger.LogInformation("TraceId: {traceId} Service: {serviceName} LogType: {logType} Method: {method} Message: {message} | Other Parameter SalesArea: {salesArea}", traceId, nameof(SalesAreaConfigurationService), nameof(LoggingTypes.IssueLog), nameof(MapSalesAreaToSalesAreaConfiguration), "No pricelist for Sales area, sales area will be skipped", salesArea);
                    return null;
                }


                var salesAreaConfig = new SalesAreaConfiguration
                {
                    SalesAreaId = salesArea.SalesAreaId,
                    SalesAreaCode = salesArea.Code,
                    CurrencyCode = GetCurrencyCodeForSalesArea(salesArea, salesAreaPriceLists, currencies, _logger, traceId),
                    PriceListIds = salesAreaPriceLists.Select(x => x.PriceListId).ToList(),
                    IsPrimary = salesArea.IsPrimary,
                    Created = salesArea.Created,
                    Updated = salesArea.Updated
                };

                return salesAreaConfig;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "TraceId: {traceId} Service: {serviceName} LogType: {logType} Method: {method} Error Source: {errorSource} Error Message: {errorMessage} Error Stacktrace: {errorStackTrace} Error Inner Exception: {errorInnerException} Internal Message: {internalMessage}| Other Parameters SalesArea: {salesArea}", traceId, nameof(CultureConfigurationService), nameof(LoggingTypes.ErrorLog), nameof(MapSalesAreaToSalesAreaConfiguration), ex.Source, ex.Message, ex.StackTrace, ex.InnerException, "An unexpected error occurred while mapping MapSalesAreaToSalesAreaConfiguration", salesArea);
                throw ex;
            }
        }

        /// <summary>
        /// Retrieves the appropriate currency code for a sales area based on its associated pricelists.
        /// </summary>
        /// <param name="salesArea">The sales area to get currency for</param>
        /// <param name="salesAreaPriceLists">Price lists associated with the sales area</param>
        /// <param name="currencies">List of all available currencies</param>
        /// <param name="traceId">Unique identifier for request tracing</param>
        /// <returns>The currency code for the sales area</returns>
        /// <exception cref="ArgumentNullException">Thrown when currency information cannot be found</exception>
        /// <exception cref="InvalidOperationException">Thrown when unable to determine the currency code</exception>
        /// <remarks>
        /// Uses the currency from the first pricelist associated with the sales area.
        /// This assumes that all pricelists for a sales area use the same currency.
        /// </remarks>
        public static string? GetCurrencyCodeForSalesArea(SalesAreaResponse salesArea, List<PriceListClientResponse> salesAreaPriceLists, List<CurrenciesResponse> currencies, ILogger _logger, Guid? traceId = null)
        {
            try
            {
                var currencyCode = salesAreaPriceLists.FirstOrDefault()?.CurrencyId;

                var currencyInfo = currencies.FirstOrDefault(x => x.Id == currencyCode);
                if (currencyInfo == null)
                    throw new ArgumentNullException(nameof(currencyInfo));

                return currencyInfo.Code;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "TraceId: {traceId} Service: {serviceName} LogType: {logType} Method: {method} Error Source: {errorSource} Error Message: {errorMessage} Error Stacktrace: {errorStackTrace} Error Inner Exception: {errorInnerException} Internal Message: {internalMessage}| Other Parameters SalesArea: {salesArea}", traceId, nameof(CultureConfigurationService), nameof(LoggingTypes.ErrorLog), nameof(GetCurrencyCodeForSalesArea), ex.Source, ex.Message, ex.StackTrace, ex.InnerException, "An unexpected error occurred while getting GetCurrencyCodeForSalesArea for sales area", salesArea);
                throw ex;
            }
        }

        /// <summary>
        /// Filters a list of sales areas based on the included sales area IDs.
        /// </summary>
        /// <param name="salesAreas">List of sales areas to filter</param>
        /// <param name="includedSalesAreaIds">List of sales area IDs to include in the result. If empty, returns all sales areas.</param>
        /// <param name="_logger">Logger instance for tracking method execution</param>
        /// <param name="traceId">Optional trace ID for logging and debugging purposes.</param>
        /// <returns>A filtered list of sales areas that match the included sales area IDs</returns>
        /// <exception cref="ArgumentNullException">Thrown when salesAreas parameter is null</exception>
        public static List<SalesAreaResponse> FilterSalesAreasByIncludedIds(List<SalesAreaResponse> salesAreas, List<int> includedSalesAreaIds, ILogger _logger, Guid? traceId = null)
        {
            try
            {
                if (salesAreas == null)
                    throw new ArgumentNullException(nameof(salesAreas));

                if (includedSalesAreaIds == null || !includedSalesAreaIds.Any())
                {
                    _logger.LogInformation(
                        "TraceId: {traceId} Service: {serviceName} LogType: {logType} Method: {method} Message: {message}",
                        traceId,
                        nameof(SalesAreaConfigurationServiceExtension),
                        nameof(LoggingTypes.CheckpointLog),
                        nameof(FilterSalesAreasByIncludedIds),
                        "No sales area IDs filter specified, returning all sales areas"
                    );
                    return salesAreas;
                }

                var filteredSalesAreas = salesAreas
                    .Where(sa => includedSalesAreaIds.Contains(sa.SalesAreaId))
                    .ToList();

                _logger.LogInformation(
                    "TraceId: {traceId} Service: {serviceName} LogType: {logType} Method: {method} Message: {message} | Other Parameters TotalSalesAreas: {totalSalesAreas}, FilteredSalesAreas: {filteredSalesAreas}, FilteredSalesAreasCodes: {filteredSalesAreasCodes}",
                    traceId,
                    nameof(SalesAreaConfigurationServiceExtension),
                    nameof(LoggingTypes.CheckpointLog),
                    nameof(FilterSalesAreasByIncludedIds),
                    "Successfully filtered sales areas",
                    salesAreas.Count,
                    filteredSalesAreas.Count,
                    string.Join(',',filteredSalesAreas.Select(x => x.Code))
                );

                return filteredSalesAreas;
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "TraceId: {traceId} Service: {serviceName} LogType: {logType} Method: {method} Error Source: {errorSource} Error Message: {errorMessage} Error Stacktrace: {errorStackTrace} Error Inner Exception: {errorInnerException} Internal Message: {internalMessage}",
                    traceId,
                    nameof(SalesAreaConfigurationServiceExtension),
                    nameof(LoggingTypes.ErrorLog),
                    nameof(FilterSalesAreasByIncludedIds),
                    ex.Source,
                    ex.Message,
                    ex.StackTrace,
                    ex.InnerException,
                    "An unexpected error occurred while filtering sales areas"
                );
                throw;
            }
        }
    }
}
