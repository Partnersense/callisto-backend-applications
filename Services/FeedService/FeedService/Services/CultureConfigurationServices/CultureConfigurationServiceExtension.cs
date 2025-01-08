using FeedService.Domain.Models;
using SharedLib.Logging.Enums;
using SharedLib.Models.Norce.Query;

namespace FeedService.Services.CultureConfigurationServices
{
    public static class CultureConfigurationServiceExtension
    {
        /// <summary>
        /// Maps a Norce client culture to a market configuration by extracting the market code from the culture code.
        /// </summary>
        /// <param name="culture">The Norce client culture containing the culture code to map.</param>
        /// <param name="_logger">Logger instance for tracking method execution</param>
        /// <param name="traceId">Optional trace ID for logging and debugging purposes.</param>
        /// <returns>
        /// A task that represents the asynchronous operation. The task result contains:
        /// - A <see cref="CultureConfiguration"/> if the mapping is successful
        /// - Null if the culture code format is invalid (not in the format "xx-XX")
        /// </returns>
        /// <exception cref="InvalidOperationException">
        /// Thrown when an unexpected error occurs during the mapping process.
        /// </exception>
        public static CultureConfiguration? MapCultureToMarketConfiguration(ClientCultureResponse culture, ILogger _logger, Guid? traceId = null)
        {
            try
            {
                if (culture?.CultureCode == null)
                    throw new InvalidOperationException($"Invalid culture code format: {culture?.CultureCode}. Expected format: xx-XX");

                var parts = culture.CultureCode.Split('-');
                if (parts.Length != 2)
                    throw new InvalidOperationException($"Invalid culture code format: {culture.CultureCode}. Expected format: xx-XX");


                var marketCode = parts[1];
                var configuration = new CultureConfiguration
                {
                    MarketCode = marketCode,
                    CultureCode = culture.CultureCode
                };

                _logger.LogInformation(
                    "TraceId: {traceId} Service: {serviceName} LogType: {logType} Method: {method} Message: {message} | Other Parameters CultureCode: {cultureCode}, MarketCode: {marketCode}",
                    traceId,
                    nameof(CultureConfigurationServiceExtension),
                    nameof(LoggingTypes.CheckpointLog),
                    nameof(MapCultureToMarketConfiguration),
                    "Successfully mapped culture to market configuration",
                    culture.CultureCode,
                    marketCode
                );


                return configuration;
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "TraceId: {traceId} Service: {serviceName} LogType: {logType} Method: {method} Error Source: {errorSource} Error Message: {errorMessage} Error Stacktrace: {errorStackTrace} Error Inner Exception: {errorInnerException} Internal Message: {internalMessage} | Other Parameter CultureCode: {cultureCode}",
                    traceId,
                    nameof(CultureConfigurationService),
                    nameof(LoggingTypes.ErrorLog),
                    nameof(MapCultureToMarketConfiguration),
                    ex.Source,
                    ex.Message,
                    ex.StackTrace,
                    ex.InnerException,
                    "An unexpected error occurred while mapping culture to market configuration",
                    culture?.CultureCode
                );
                throw ex;
            }
        }

        /// <summary>
        /// Filters a list of client cultures based on the included culture codes.
        /// </summary>
        /// <param name="cultures">List of client cultures to filter</param>
        /// <param name="includedCultureCodes">List of culture codes to include in the result. If empty, returns all cultures.</param>
        /// <param name="_logger">Logger instance for tracking method execution</param>
        /// <param name="traceId">Optional trace ID for logging and debugging purposes.</param>
        /// <returns>A filtered list of client cultures that match the included culture codes</returns>
        /// <exception cref="ArgumentNullException">Thrown when cultures parameter is null</exception>
        public static List<ClientCultureResponse> FilterCulturesByIncludedCodes(List<ClientCultureResponse> cultures, List<string> includedCultureCodes, ILogger _logger, Guid? traceId)
        {
            try
            {
                if (cultures == null)
                    throw new ArgumentNullException(nameof(cultures));


                if (includedCultureCodes == null || !includedCultureCodes.Any())
                {
                    _logger.LogInformation(
                        "TraceId: {traceId} Service: {serviceName} LogType: {logType} Method: {method} Message: {message}",
                        traceId,
                        nameof(CultureConfigurationServiceExtension),
                        nameof(LoggingTypes.CheckpointLog),
                        nameof(FilterCulturesByIncludedCodes),
                        "No culture codes filter specified, returning all cultures"
                    );
                    return cultures;
                }

                var filteredCultures = cultures
                    .Where(c => includedCultureCodes.Contains(c.CultureCode, StringComparer.OrdinalIgnoreCase))
                    .ToList();

                _logger.LogInformation(
                    "TraceId: {traceId} Service: {serviceName} LogType: {logType} Method: {method} Message: {message} | Other Parameters TotalCultures: {totalCultures}, FilteredCultures: {filteredCultures}, FilteredCulturesCodes: {filteredCulturesCodes}",
                    traceId,
                    nameof(CultureConfigurationServiceExtension),
                    nameof(LoggingTypes.CheckpointLog),
                    nameof(FilterCulturesByIncludedCodes),
                    "Successfully filtered cultures",
                    cultures.Count,
                    filteredCultures.Count,
                    string.Join(',', filteredCultures.Select(x => x.CultureCode))
                );

                return filteredCultures;
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "TraceId: {traceId} Service: {serviceName} LogType: {logType} Method: {method} Error Source: {errorSource} Error Message: {errorMessage} Error Stacktrace: {errorStackTrace} Error Inner Exception: {errorInnerException} Internal Message: {internalMessage}",
                    traceId,
                    nameof(CultureConfigurationServiceExtension),
                    nameof(LoggingTypes.ErrorLog),
                    nameof(FilterCulturesByIncludedCodes),
                    ex.Source,
                    ex.Message,
                    ex.StackTrace,
                    ex.InnerException,
                    "An unexpected error occurred while filtering cultures"
                );
                throw ex;
            }
        }
    }
}
