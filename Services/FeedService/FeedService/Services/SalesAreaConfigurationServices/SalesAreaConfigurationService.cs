using Enferno.Services.StormConnect.Contracts.Product.Models;
using FeedService.Domain.Models;
using FeedService.Domain.Norce;
using FeedService.Services.CultureConfigurationServices;
using Microsoft.Extensions.Options;
using SharedLib.Clients.Norce;
using SharedLib.Helpers.Norce;
using SharedLib.Logging.Enums;
using SharedLib.Models.Norce.Query;
using SharedLib.Options.Models;
using System.Diagnostics;
using System.Text.Json;

namespace FeedService.Services.SalesAreaConfigurationServices
{
    public class SalesAreaConfigurationService(
        ILogger<SalesAreaConfigurationService> logger,
        INorceClient _norceClient,
        IOptionsMonitor<NorceBaseModuleOptions> _norceOptions, IOptionsMonitor<BaseModuleOptions> _baseOptions) : ISalesAreaConfigurationService
    {
        public async Task<List<SalesAreaConfiguration>> GetSalesAreaConfigurations(Guid? traceId = null)
        {
            try
            {
                //Get sales area information
                var salesAreasResponse = await _norceClient.Query.GetAsync(Endpoints.Query.Application.ClientSalesAreas);
                if ( string.IsNullOrEmpty(salesAreasResponse))
                    throw new ArgumentNullException(nameof(salesAreasResponse));

                var salesAreas = ODataHelper.DeserializeODataResponse<SalesAreaResponse>(salesAreasResponse,logger,traceId);
                if (salesAreas == null || !salesAreas.Any())
                    throw new ArgumentNullException(nameof(salesAreas));


                //Get pricelist information
                var priceListsResponse = await _norceClient.Query.GetAsync(Endpoints.Query.Application.ClientPriceListsIncPriceList);
                if (string.IsNullOrEmpty(priceListsResponse))
                    throw new ArgumentNullException(nameof(priceListsResponse));

                var priceLists = ODataHelper.DeserializeODataResponse < PriceListClientResponse>(priceListsResponse,logger, traceId);
                if (priceLists == null || !priceLists.Any())
                    throw new ArgumentNullException(nameof(priceLists));


                //Get currencies information
                var currenciesResponse = await _norceClient.Query.GetAsync(Endpoints.Query.Core.Currencies);
                if (string.IsNullOrEmpty(currenciesResponse))
                    throw new ArgumentNullException(nameof(currenciesResponse));

                var currencies = ODataHelper.DeserializeODataResponse<CurrenciesResponse>(currenciesResponse, logger,traceId);
                if (currencies == null || !currencies.Any())
                    throw new ArgumentNullException(nameof(currencies));


                var filteredSalesArea = SalesAreaConfigurationServiceExtension.FilterSalesAreasByIncludedIds(salesAreas, _baseOptions.CurrentValue.IncludedSalesAreaIdsList, logger, traceId);

                var salesAreaConfigs = new List<SalesAreaConfiguration>();

                foreach (var salesArea in filteredSalesArea)
                {
                    var config = SalesAreaConfigurationServiceExtension.MapSalesAreaToSalesAreaConfiguration(salesArea, priceLists, currencies, logger, traceId);
                    if (config != null)
                    {
                        salesAreaConfigs.Add(config);
                    }
                }

                return salesAreaConfigs;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "TraceId: {traceId} Service: {serviceName} LogType: {logType} Error Source: {errorSource} Error Message: {errorMessage} Error Stacktrace: {errorStackTrace} Error Inner Exception: {errorInnerException} Internal Message: {internalMessage}| Other Parameters", traceId, nameof(CultureConfigurationService), nameof(LoggingTypes.ErrorLog), ex.Source, ex.Message, ex.StackTrace, ex.InnerException, "An unexpected error occurred while retrieving market configurations");
                throw ex;
            }
        }
        
    }
}
