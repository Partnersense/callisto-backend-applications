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
        IOptionsMonitor<NorceBaseModuleOptions> _norceOptions,
        IOptionsMonitor<BaseModuleOptions> _baseOptions) : ICultureFeedGenerationService
    {
        public async Task<List<object>> GenerateFeedWithCultures(List<CultureConfiguration> cultures, Guid? traceId = null)
        {

            // This method should include minimal logic see other classes like SalesAreaConfigurationService.cs class that class and primary method only does requests and sends the logic to a static class in this cas the CultureFeedGenerationServiceExtension class,
            // I think this is a neat way to keep logic from request, easier tested an in my opinion easier to maintain and read.  As the logic get to be pure logic. 

            //The tests should be focused on the logic in the extension class, every method in the extension class needs test and docs very important for quality, readability and maintainability.
            //Test with moqs for this class to test stuff like error handling of wrongfully requests. See example of test for service class and extension class in FeedServiceTests\Services\CultureConfigurationsServicesTests
            throw new NotImplementedException();
        }
    }
}
