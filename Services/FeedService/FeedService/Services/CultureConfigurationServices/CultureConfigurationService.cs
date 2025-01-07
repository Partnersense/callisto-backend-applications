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

namespace FeedService.Services.CultureConfigurationServices
{
    public class CultureConfigurationService(
        ILogger<CultureConfigurationService> _logger,
        INorceClient _norceClient,
        IOptionsMonitor<BaseModuleOptions> baseOptions,
        IOptionsMonitor<NorceBaseModuleOptions> _norceOptions) : ICultureConfigurationService
    {

        public async Task<List<CultureConfiguration>> GetCultureConfigurations(Guid? traceId = null)
        {
            try
            {
                var culturesResponse = await _norceClient.Query.GetAsync(Endpoints.Query.Application.ClientCultures);
                if (string.IsNullOrEmpty(culturesResponse))
                    throw new ArgumentNullException(nameof(culturesResponse));

                var cultures = JsonSerializer.Deserialize<List<ClientCultureResponse>>(culturesResponse);
                if (cultures == null || !cultures.Any())
                    throw new ArgumentNullException(nameof(cultures));


                //filter
                var filteredCultures = CultureConfigurationServiceExtension.FilterCulturesByIncludedCodes(cultures, baseOptions.CurrentValue.IncludedCulturesList, _logger, traceId);

                var marketConfigs = new List<CultureConfiguration>();

                foreach (var culture in filteredCultures)
                {
                    var config = CultureConfigurationServiceExtension.MapCultureToMarketConfiguration(culture, _logger, traceId);
                    if (config != null)
                    {
                        marketConfigs.Add(config);
                    }
                }

                return marketConfigs;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "TraceId: {traceId} Service: {serviceName} LogType: {logType} Error Source: {errorSource} Error Message: {errorMessage} Error Stacktrace: {errorStackTrace} Error Inner Exception: {errorInnerException} Internal Message: {internalMessage}| Other Parameters", traceId, nameof(CultureConfigurationService), nameof(LoggingTypes.ErrorLog), ex.Source, ex.Message, ex.StackTrace, ex.InnerException, "An unexpected error occurred while retrieving market configurations");
                throw ex;
            }
        }

    }
}
