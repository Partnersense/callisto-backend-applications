using FeedService.Services.CultureConfigurationServices;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using SharedLib.Clients.Norce;
using SharedLib.Models.Norce.Query;
using SharedLib.Options.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace FeedServiceTests.Services.CultureConfigurationsServicesTests
{
    public class CultureConfigurationServiceTests
    {
        private readonly Mock<ILogger<CultureConfigurationService>> _loggerMock;
        private readonly Mock<INorceClient> _norceClientMock;
        private readonly Mock<INorceQueryClient> _queryClientMock;
        private readonly Mock<IOptionsMonitor<NorceBaseModuleOptions>> _optionsMonitorMock;
        private readonly CultureConfigurationService _service;

        public CultureConfigurationServiceTests()
        {
            _loggerMock = new Mock<ILogger<CultureConfigurationService>>();
            _norceClientMock = new Mock<INorceClient>();
            _queryClientMock = new Mock<INorceQueryClient>();
            _optionsMonitorMock = new Mock<IOptionsMonitor<NorceBaseModuleOptions>>();

            // Setup Query property to return the mock query client
            _norceClientMock.Setup(x => x.Query).Returns(_queryClientMock.Object);

            _service = new CultureConfigurationService(
                _loggerMock.Object,
                _norceClientMock.Object,
                _optionsMonitorMock.Object
            );
        }

        [Fact]
        public async Task GetCultureConfigurations_Success_ReturnsMarketConfigurations()
        {
            // Arrange
            var traceId = Guid.NewGuid();
            var cultures = new List<ClientCultureResponse>
            {
                new ClientCultureResponse
                {
                    CultureCode = "en-US",
                    ClientId = 1,
                    IsPrimary = true,
                    Created = DateTime.UtcNow,
                    CreatedBy = 1
                }
            };

            var jsonResponse = JsonSerializer.Serialize(cultures);
            _queryClientMock.Setup(x => x.GetAsync(Endpoints.Query.Application.ClientCultures, It.IsAny<string>()))
                .ReturnsAsync(jsonResponse);

            // Act
            var result = await _service.GetCultureConfigurations(traceId);

            // Assert
            Assert.NotNull(result);
            Assert.Single(result);
            var config = result.First();
            Assert.Equal("US", config.MarketCode);
            Assert.Equal("en-US", config.CultureCode);
        }

        [Fact]
        public async Task GetCultureConfigurations_NullApiResponse_ThrowsArgumentNullException()
        {
            // Arrange
            var traceId = Guid.NewGuid();
            _queryClientMock.Setup(x => x.GetAsync(Endpoints.Query.Application.ClientCultures, It.IsAny<string>()))
                .ReturnsAsync((string)null);

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() =>
                _service.GetCultureConfigurations(traceId));
        }

        [Fact]
        public async Task GetCultureConfigurations_NullDeserializedResponse_ThrowsArgumentNullException()
        {
            // Arrange
            var traceId = Guid.NewGuid();
            _queryClientMock.Setup(x => x.GetAsync(Endpoints.Query.Application.ClientCultures, It.IsAny<string>()))
                .ReturnsAsync("[]"); // Empty JSON response

            // Act & Assert  
            await Assert.ThrowsAsync<ArgumentNullException>(() =>
                _service.GetCultureConfigurations(traceId));
        }

        [Fact]
        public async Task GetCultureConfigurations_InvalidCultureCode__ThrowsArgumentInvalidOperationException()
        {
            // Arrange
            var traceId = Guid.NewGuid();
            var cultures = new List<ClientCultureResponse>
            {
                new ClientCultureResponse
                {
                    CultureCode = "invalid", // Invalid culture code format
                    ClientId = 1,
                    IsPrimary = true,
                    Created = DateTime.UtcNow,
                    CreatedBy = 1
                }
            };

            var jsonResponse = JsonSerializer.Serialize(cultures);
            _queryClientMock.Setup(x => x.GetAsync(Endpoints.Query.Application.ClientCultures, It.IsAny<string>()))
                .ReturnsAsync(jsonResponse);

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() =>
                _service.GetCultureConfigurations(traceId));
        }

        [Fact]
        public async Task GetCultureConfigurations_ApiException_ThrowsHttpRequestException()
        {
            // Arrange
            var traceId = Guid.NewGuid();
            _queryClientMock.Setup(x => x.GetAsync(Endpoints.Query.Application.ClientCultures, It.IsAny<string>()))
                .ThrowsAsync(new HttpRequestException("API Error"));

            // Act & Assert
            var exception = await Assert.ThrowsAsync<HttpRequestException>(
                () => _service.GetCultureConfigurations(traceId));
            Assert.Equal("API Error", exception.Message);
        }
    }
}
