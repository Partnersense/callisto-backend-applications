using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using SharedLib.Clients.Norce;
using SharedLib.Constants.NorceConstants;
using SharedLib.Constants;
using SharedLib.Options.Models;
using FeedService.Domain.Models;
using System.Text.Json;
using SharedLib.Models.Norce.Query;
using Enferno.Services.StormConnect.Contracts.Product.Models;
using SharedLib.Logging.Enums;
using System.Diagnostics;

namespace FeedService.Services
{
    public class CultureConfigurationService(
        ILogger<CultureConfigurationService> _logger,
        INorceClient _norceClient,
        IOptionsMonitor<NorceBaseModuleOptions> _norceOptions) : ICultureConfigurationService
    {

        public async Task<List<MarketConfiguration>> GetCultureConfigurations(Guid? traceId = null)
        {
            try
            {
                var query = await _norceClient.Query.GetAsync(Endpoints.Query.Application.ClientCultures);
                var cultures = JsonSerializer.Deserialize<List<ClientCultureResponse>>(query);

                var marketConfigs = new List<MarketConfiguration>();

                foreach (var culture in cultures!)
                {
                    var config = await MapCultureToMarketConfiguration(culture);
                    if (config != null)
                    {
                        marketConfigs.Add(config);
                    }
                }

                return marketConfigs;
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "TraceId: {traceId} Service: {serviceName} LogType: {logType} Error Source: {errorSource} Error Message: {errorMessage} Error Stacktrace: {errorStackTrace} Error Inner Exception: {errorInnerException} Internal Message: {internalMessage}| Other Parameters", traceId, nameof(CultureConfigurationService), nameof(LoggingTypes.ErrorLog), ex.Source, ex.Message, ex.StackTrace, ex.InnerException, "Failed to retrieve market configurations from Norce API");
                throw new InvalidOperationException("Failed to retrieve market configurations from Norce API", ex);
            }
            catch (JsonException ex)
            {
                _logger.LogError(ex, "TraceId: {traceId} Service: {serviceName} LogType: {logType} Error Source: {errorSource} Error Message: {errorMessage} Error Stacktrace: {errorStackTrace} Error Inner Exception: {errorInnerException} Internal Message: {internalMessage}| Other Parameters", traceId, nameof(CultureConfigurationService), nameof(LoggingTypes.ErrorLog), ex.Source, ex.Message, ex.StackTrace, ex.InnerException, "Failed to deserialize market configurations");
                throw new InvalidOperationException("Failed to parse market configuration data", ex);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "TraceId: {traceId} Service: {serviceName} LogType: {logType} Error Source: {errorSource} Error Message: {errorMessage} Error Stacktrace: {errorStackTrace} Error Inner Exception: {errorInnerException} Internal Message: {internalMessage}| Other Parameters", traceId, nameof(CultureConfigurationService), nameof(LoggingTypes.ErrorLog), ex.Source, ex.Message, ex.StackTrace, ex.InnerException, "An unexpected error occurred while retrieving market configurations");
                throw new InvalidOperationException("An unexpected error occurred while retrieving market configurations", ex);
            }
        }

        /// <summary>
        /// Maps a Norce client culture to a market configuration by extracting the market code from the culture code.
        /// </summary>
        /// <param name="culture">The Norce client culture containing the culture code to map.</param>
        /// <param name="traceId">Optional trace ID for logging and debugging purposes.</param>
        /// <returns>
        /// A task that represents the asynchronous operation. The task result contains:
        /// - A <see cref="MarketConfiguration"/> if the mapping is successful
        /// - Null if the culture code format is invalid (not in the format "xx-XX")
        /// </returns>
        /// <exception cref="InvalidOperationException">
        /// Thrown when an unexpected error occurs during the mapping process.
        /// </exception>
        private Task<MarketConfiguration?> MapCultureToMarketConfiguration(ClientCultureResponse culture, Guid? traceId = null)
        {
            try
            {
                var parts = culture.CultureCode.Split('-');
                if (parts.Length != 2)
                {
                    throw new InvalidOperationException($"Invalid culture code format: {culture.CultureCode}. Expected format: xx-XX");
                }

                var marketCode = parts[1];

                return Task.FromResult<MarketConfiguration?>(new MarketConfiguration
                {
                    MarketCode = marketCode,
                    CultureCode = culture.CultureCode
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "TraceId: {traceId} Service: {serviceName} LogType: {logType} Method: {method} Error Source: {errorSource} Error Message: {errorMessage} Error Stacktrace: {errorStackTrace} Error Inner Exception: {errorInnerException} Internal Message: {internalMessage}| Other Parameters {cultureCode}", traceId, nameof(CultureConfigurationService), nameof(LoggingTypes.ErrorLog), nameof(MapCultureToMarketConfiguration), ex.Source, ex.Message, ex.StackTrace, ex.InnerException, "An unexpected error occurred while mapping ClientCulture to MarketConfiguration", culture.CultureCode);
                throw new InvalidOperationException("An unexpected error occurred while mapping ClientCulture to MarketConfiguration", ex);
            }
        }
        
    }
}
