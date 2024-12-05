using Enferno.Services.StormConnect.Contracts.Product.Models;
using FeedService.Domain.Models;
using FeedService.Domain.Norce;
using FeedService.Services.CultureConfigurationServices;
using Microsoft.Extensions.Options;
using SharedLib.Clients.Norce;
using SharedLib.Logging.Enums;
using SharedLib.Models.Norce.Query;
using SharedLib.Options.Models;
using System.Diagnostics;
using System.Text.Json;

namespace FeedService.Services.SalesAreaConfigurationServices
{
    public class SalesAreaConfigurationService(
        ILogger<SalesAreaConfigurationService> _logger,
        INorceClient _norceClient,
        IOptionsMonitor<NorceBaseModuleOptions> _norceOptions) : ISalesAreaConfigurationService
    {
        public async Task<List<SalesAreaConfiguration>> GetSalesAreaConfigurations(Guid? traceId = null)
        {
            try
            {
                //Get sales area information
                var salesAreasResponse = await _norceClient.Query.GetAsync(Endpoints.Query.Application.ClientSalesAreas);
                if ( string.IsNullOrEmpty(salesAreasResponse))
                    throw new ArgumentNullException(nameof(salesAreasResponse));

                var salesAreas = JsonSerializer.Deserialize<List<SalesAreaResponse>>(salesAreasResponse);
                if (salesAreas == null || !salesAreas.Any())
                    throw new ArgumentNullException(nameof(salesAreas));


                //Get pricelist information
                var priceListsResponse = await _norceClient.Query.GetAsync(Endpoints.Query.Application.ClientPriceLists);
                if (string.IsNullOrEmpty(priceListsResponse))
                    throw new ArgumentNullException(nameof(priceListsResponse));

                var priceLists = JsonSerializer.Deserialize<List<PriceListClientResponse>>(priceListsResponse);
                if (priceLists == null || !priceLists.Any())
                    throw new ArgumentNullException(nameof(priceLists));


                //Get currencies information
                var currenciesResponse = await _norceClient.Query.GetAsync(Endpoints.Query.Core.Currencies);
                if (string.IsNullOrEmpty(currenciesResponse))
                    throw new ArgumentNullException(nameof(currenciesResponse));

                var currencies = JsonSerializer.Deserialize<List<CurrenciesResponse>>(currenciesResponse);
                if (currencies == null || !currencies.Any())
                    throw new ArgumentNullException(nameof(currencies));



                var salesAreaConfigs = new List<SalesAreaConfiguration>();

                foreach (var salesArea in salesAreas)
                {
                    var config = SalesAreaConfigurationServiceExtension.MapSalesAreaToSalesAreaConfiguration(salesArea, priceLists, currencies, _logger, traceId);
                    if (config != null)
                    {
                        salesAreaConfigs.Add(config);
                    }
                }

                return salesAreaConfigs;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "TraceId: {traceId} Service: {serviceName} LogType: {logType} Error Source: {errorSource} Error Message: {errorMessage} Error Stacktrace: {errorStackTrace} Error Inner Exception: {errorInnerException} Internal Message: {internalMessage}| Other Parameters", traceId, nameof(CultureConfigurationService), nameof(LoggingTypes.ErrorLog), ex.Source, ex.Message, ex.StackTrace, ex.InnerException, "An unexpected error occurred while retrieving market configurations");
                throw ex;
            }
        }
        
    }
}
