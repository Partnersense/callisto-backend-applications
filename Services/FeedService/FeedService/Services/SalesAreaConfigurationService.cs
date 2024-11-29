using Enferno.Services.StormConnect.Contracts.Product.Models;
using FeedService.Domain.Models;
using Microsoft.Extensions.Options;
using SharedLib.Clients.Norce;
using SharedLib.Logging.Enums;
using SharedLib.Models.Norce.Query;
using SharedLib.Options.Models;
using System.Text.Json;

namespace FeedService.Services
{
    public class SalesAreaConfigurationService(
        ILogger<CultureConfigurationService> _logger,
        INorceClient _norceClient,
        IOptionsMonitor<NorceBaseModuleOptions> _norceOptions)
    {
        public async Task<List<SalesAreaConfiguration>> GetCultureConfigurations(Guid? traceId = null)
        {
            try
            {
                //Get sales area information
                var salesAreasResponse = await _norceClient.Query.GetAsync(Endpoints.Query.Application.ClientSalesAreas);
                if (salesAreasResponse == null)
                    throw new ArgumentNullException(nameof(salesAreasResponse));

                var salesAreas = JsonSerializer.Deserialize<List<SalesAreaResponse>>(salesAreasResponse);
                if (salesAreas == null)
                    throw new ArgumentNullException(nameof(salesAreas));


                //Get pricelist information
                var priceListsResponse = await _norceClient.Query.GetAsync(Endpoints.Query.Application.ClientPriceLists);
                if (priceListsResponse == null)
                    throw new ArgumentNullException(nameof(priceListsResponse));

                var priceLists = JsonSerializer.Deserialize<List<PriceListClientResponse>>(priceListsResponse);
                if (priceLists == null)
                    throw new ArgumentNullException(nameof(priceLists));


                var salesAreaConfigs = new List<SalesAreaConfiguration>();


                foreach (var salesArea in salesAreas)
                {
                    var config = await MapSalesAreaToSalesAreaConfiguration(salesArea, priceLists, traceId);
                    if (config != null)
                    {
                        salesAreaConfigs.Add(config);
                    }
                }

                return salesAreaConfigs;
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

       
        private Task<SalesAreaConfiguration?> MapSalesAreaToSalesAreaConfiguration(SalesAreaResponse salesArea, List<PriceListClientResponse> allPriceLists, Guid? traceId = null)
        {
            try
            {    
                // Get pricelists for this sales area
                var salesAreaPriceLists = allPriceLists
                    .Where(pl => pl.PriceList.IsActive && pl.SalesAreaId == salesArea.SalesAreaId)
                    .Select(pl => pl.PriceListId)
                    .ToList();

                var config = new SalesAreaConfiguration
                {
                    SalesAreaId = salesArea.SalesAreaId,
                    SalesAreaCode = salesArea.Code,
                    CurrencyCode = GetCurrencyCodeForSalesArea(salesArea.Code), // You'll need to implement this method
                    PriceListCodes = salesAreaPriceLists,
                    IsPrimary = salesArea.IsPrimary,
                    Created = salesArea.Created,
                    Updated = salesArea.Updated
                };

                return Task.FromResult<SalesAreaConfiguration?>(config);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "TraceId: {traceId} Service: {serviceName} LogType: {logType} Method: {method} Error Source: {errorSource} Error Message: {errorMessage} Error Stacktrace: {errorStackTrace} Error Inner Exception: {errorInnerException} Internal Message: {internalMessage}| Other Parameters {cultureCode}", traceId, nameof(CultureConfigurationService), nameof(LoggingTypes.ErrorLog), nameof(MapSalesAreaToSalesAreaConfiguration), ex.Source, ex.Message, ex.StackTrace, ex.InnerException, "An unexpected error occurred while mapping ClientCulture to MarketConfiguration", culture.CultureCode);
                throw new InvalidOperationException("An unexpected error occurred while mapping ClientCulture to MarketConfiguration", ex);
            }
        }
    }
}
