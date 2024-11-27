using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using SharedLib.Clients.Norce;
using SharedLib.Constants.NorceConstants;
using SharedLib.Constants;
using SharedLib.Options.Models;
using FeedService.Domain.Models;
using System.Text.Json;
using SharedLib.Models.Norce.Query;

namespace FeedService.Services
{
    public class MarketConfigurationService(
        ILogger<MarketConfigurationService> logger,
        INorceClient norceClient,
        IMemoryCache cache,
        IOptionsMonitor<NorceBaseModuleOptions> norceOptions) : IMarketConfigurationService
    {
        private static readonly TimeSpan CacheDuration = TimeSpan.FromMinutes(15);

        public async Task<IEnumerable<MarketConfiguration>> GetMarketConfigurations()
        {
            try
            {
                // Fetch market configurations from Norce
                var query = await norceClient.Query.GetAsync(Endpoints.Query.Application.ClientCultures);
                var cultures = JsonSerializer.Deserialize<List<ClientCulture>>(query);

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
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to fetch market configurations from Norce");
                throw
            }
        }

        private async Task<MarketConfiguration?> MapCultureToMarketConfiguration(ClientCulture culture)
        {
            try
            {

                return new MarketConfiguration
                {
                    MarketCode = culture.CultureCode,
                    CultureCode = culture.CultureCode,
                    PriceListCode = priceList?.Code ?? GetDefaultPriceListCode(culture.Code),
                    CurrencyCode = priceList?.CurrencyCode ?? GetDefaultCurrencyCode(culture.Code),
                    IsActive = culture.IsActive
                };
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to map culture {Code} to market configuration", culture.Code);
                return null;
            }
        }
        
    }
}
