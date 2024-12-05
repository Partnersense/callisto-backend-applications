using FeedService.Domain.Models;
using FeedService.Services.SalesAreaConfigurationServices;
using Microsoft.Extensions.Options;
using SharedLib.Clients.Norce;
using SharedLib.Options.Models;

namespace FeedService.Services.CultureFeedGenerationServices
{
    public class CultureFeedGenerationService(
        ILogger<CultureFeedGenerationService> _logger,
        INorceClient _norceClient,
        IOptionsMonitor<NorceBaseModuleOptions> _norceOptions) : ICultureFeedGenerationService
    {
        public async Task<List<MarketFeed>> GenerateFeedWithCultures(List<CultureConfiguration> cultures, Guid? traceId = null)
        {
            throw new NotImplementedException();
        }
    }
}
