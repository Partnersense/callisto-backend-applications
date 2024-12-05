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
        /// <param name="traceId">Optional trace ID for logging and debugging purposes.</param>
        /// <returns>
        /// A task that represents the asynchronous operation. The task result contains:
        /// - A <see cref="CultureConfiguration"/> if the mapping is successful
        /// - Null if the culture code format is invalid (not in the format "xx-XX")
        /// </returns>
        /// <exception cref="InvalidOperationException">
        /// Thrown when an unexpected error occurs during the mapping process.
        /// </exception>
        public static Task<CultureConfiguration?> MapCultureToMarketConfiguration(ClientCultureResponse culture, ILogger _logger, Guid? traceId = null)
        {
            try
            {
                var parts = culture.CultureCode.Split('-');
                if (parts.Length != 2)
                     throw new InvalidOperationException($"Invalid culture code format: {culture.CultureCode}. Expected format: xx-XX");
                

                var marketCode = parts[1];

                return Task.FromResult<CultureConfiguration?>(new CultureConfiguration
                {
                    MarketCode = marketCode,
                    CultureCode = culture.CultureCode
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "TraceId: {traceId} Service: {serviceName} LogType: {logType} Method: {method} Error Source: {errorSource} Error Message: {errorMessage} Error Stacktrace: {errorStackTrace} Error Inner Exception: {errorInnerException} Internal Message: {internalMessage}| Other Parameters {cultureCode}", traceId, nameof(CultureConfigurationService), nameof(LoggingTypes.ErrorLog), nameof(MapCultureToMarketConfiguration), ex.Source, ex.Message, ex.StackTrace, ex.InnerException, "An unexpected error occurred while mapping ClientCulture to MarketConfiguration", culture.CultureCode);
                throw ex;
            }
        }
    }
}
