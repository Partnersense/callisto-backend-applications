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
        private readonly Mock<IOptionsMonitor<NorceBaseModuleOptions>> _norceOptionsMonitorMock;
        private readonly Mock<IOptionsMonitor<BaseModuleOptions>> _baseOptionsMonitorMock;
        private readonly CultureConfigurationService _service;
        private readonly BaseModuleOptions _baseOptions;

        public CultureConfigurationServiceTests()
        {
            _loggerMock = new Mock<ILogger<CultureConfigurationService>>();
            _norceClientMock = new Mock<INorceClient>();
            _queryClientMock = new Mock<INorceQueryClient>();
            _norceOptionsMonitorMock = new Mock<IOptionsMonitor<NorceBaseModuleOptions>>();
            _baseOptionsMonitorMock = new Mock<IOptionsMonitor<BaseModuleOptions>>();

            // Setup default base options with no filtering
            _baseOptions = new BaseModuleOptions
            {
                SiteUrl = "https://test.com",
                IncludedCultures = "" // Default to no filtering
            };
            _baseOptionsMonitorMock.Setup(x => x.CurrentValue).Returns(_baseOptions);

            // Setup Query property to return the mock query client
            _norceClientMock.Setup(x => x.Query).Returns(_queryClientMock.Object);

            _service = new CultureConfigurationService(
                _loggerMock.Object,
                _norceClientMock.Object,
                _baseOptionsMonitorMock.Object,
                _norceOptionsMonitorMock.Object
            );
        }

        [Fact]
        public async Task GetCultureConfigurations_Success_ReturnsCultureConfigurations()
        {
            // Arrange
            var traceId = Guid.NewGuid();
            var cultures = new List<ClientCultureResponse>
            {
                new()
                {
                    CultureCode = "en-US",
                    ClientId = 1006,
                    IsPrimary = true,
                    Created = DateTime.UtcNow,
                    CreatedBy = 1
                }
            };

            var culturesJson = JsonSerializer.Serialize(cultures);
            _queryClientMock.Setup(x => x.GetAsync(Endpoints.Query.Application.ClientCultures, It.IsAny<string>()))
                .ReturnsAsync(culturesJson);

            // Act
            var result = await _service.GetCultureConfigurations(traceId);

            // Assert
            Assert.NotNull(result);
            Assert.Single(result);
            Assert.Equal("US", result[0].MarketCode);
            Assert.Equal("en-US", result[0].CultureCode);
        }

        [Fact]
        public async Task GetCultureConfigurations_NullResponse_ThrowsArgumentNullException()
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
        public async Task GetCultureConfigurations_EmptyResponse_ThrowsArgumentNullException()
        {
            // Arrange
            var traceId = Guid.NewGuid();
            _queryClientMock.Setup(x => x.GetAsync(Endpoints.Query.Application.ClientCultures, It.IsAny<string>()))
                .ReturnsAsync(string.Empty);

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() =>
                _service.GetCultureConfigurations(traceId));
        }

        [Fact]
        public async Task GetCultureConfigurations_ApiException_PropagatesException()
        {
            // Arrange
            var traceId = Guid.NewGuid();
            _queryClientMock.Setup(x => x.GetAsync(Endpoints.Query.Application.ClientCultures, It.IsAny<string>()))
                .ThrowsAsync(new HttpRequestException("API Error"));

            // Act & Assert
            var exception = await Assert.ThrowsAsync<HttpRequestException>(() =>
                _service.GetCultureConfigurations(traceId));

            // Verify error logging
            _loggerMock.Verify(
                x => x.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => true),
                    It.IsAny<Exception>(),
                    It.Is<Func<It.IsAnyType, Exception?, string>>((v, t) => true)),
                Times.Once);
        }

        [Fact]
        public async Task GetCultureConfigurations_MultipleValidCultures_ReturnsAllCultures()
        {
            // Arrange
            var traceId = Guid.NewGuid();
            var cultures = new List<ClientCultureResponse>
            {
                new() { CultureCode = "en-US", ClientId = 1006, IsPrimary = true },
                new() { CultureCode = "sv-SE", ClientId = 1006, IsPrimary = false },
                new() { CultureCode = "nb-NO", ClientId = 1006, IsPrimary = false }
            };

            var culturesJson = JsonSerializer.Serialize(cultures);
            _queryClientMock.Setup(x => x.GetAsync(Endpoints.Query.Application.ClientCultures, It.IsAny<string>()))
                .ReturnsAsync(culturesJson);

            // Act
            var result = await _service.GetCultureConfigurations(traceId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(3, result.Count);
            Assert.Contains(result, c => c.MarketCode == "US" && c.CultureCode == "en-US");
            Assert.Contains(result, c => c.MarketCode == "SE" && c.CultureCode == "sv-SE");
            Assert.Contains(result, c => c.MarketCode == "NO" && c.CultureCode == "nb-NO");
        }

        // New tests for filtering functionality
        [Fact]
        public async Task GetCultureConfigurations_WithIncludedCultures_ReturnsOnlyIncludedCultures()
        {
            // Arrange
            var traceId = Guid.NewGuid();
            _baseOptions.IncludedCultures = "en-US|sv-SE"; // Only include these cultures

            var cultures = new List<ClientCultureResponse>
            {
                new() { CultureCode = "en-US", ClientId = 1006, IsPrimary = true },
                new() { CultureCode = "sv-SE", ClientId = 1006, IsPrimary = false },
                new() { CultureCode = "nb-NO", ClientId = 1006, IsPrimary = false } // Should be filtered out
            };

            var culturesJson = JsonSerializer.Serialize(cultures);
            _queryClientMock.Setup(x => x.GetAsync(Endpoints.Query.Application.ClientCultures, It.IsAny<string>()))
                .ReturnsAsync(culturesJson);

            // Act
            var result = await _service.GetCultureConfigurations(traceId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
            Assert.Contains(result, c => c.CultureCode == "en-US");
            Assert.Contains(result, c => c.CultureCode == "sv-SE");
            Assert.DoesNotContain(result, c => c.CultureCode == "nb-NO");
        }

        [Fact]
        public async Task GetCultureConfigurations_WithEmptyIncludedCultures_ReturnsAllCultures()
        {
            // Arrange
            var traceId = Guid.NewGuid();
            _baseOptions.IncludedCultures = ""; // Empty included cultures list

            var cultures = new List<ClientCultureResponse>
            {
                new() { CultureCode = "en-US", ClientId = 1006, IsPrimary = true },
                new() { CultureCode = "sv-SE", ClientId = 1006, IsPrimary = false },
                new() { CultureCode = "nb-NO", ClientId = 1006, IsPrimary = false }
            };

            var culturesJson = JsonSerializer.Serialize(cultures);
            _queryClientMock.Setup(x => x.GetAsync(Endpoints.Query.Application.ClientCultures, It.IsAny<string>()))
                .ReturnsAsync(culturesJson);

            // Act
            var result = await _service.GetCultureConfigurations(traceId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(3, result.Count); // All cultures included
            Assert.Contains(result, c => c.CultureCode == "en-US");
            Assert.Contains(result, c => c.CultureCode == "sv-SE");
            Assert.Contains(result, c => c.CultureCode == "nb-NO");
        }

        [Fact]
        public async Task GetCultureConfigurations_WithNonExistentIncludedCulture_ReturnsOnlyExistingCultures()
        {
            // Arrange
            var traceId = Guid.NewGuid();
            _baseOptions.IncludedCultures = "en-US|sv-SE|non-EXISTENT"; // Include non-existent culture

            var cultures = new List<ClientCultureResponse>
            {
                new() { CultureCode = "en-US", ClientId = 1006, IsPrimary = true },
                new() { CultureCode = "sv-SE", ClientId = 1006, IsPrimary = false }
            };

            var culturesJson = JsonSerializer.Serialize(cultures);
            _queryClientMock.Setup(x => x.GetAsync(Endpoints.Query.Application.ClientCultures, It.IsAny<string>()))
                .ReturnsAsync(culturesJson);

            // Act
            var result = await _service.GetCultureConfigurations(traceId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
            Assert.Contains(result, c => c.CultureCode == "en-US");
            Assert.Contains(result, c => c.CultureCode == "sv-SE");
        }

        [Fact]
        public async Task GetCultureConfigurations_WithNullIncludedCultures_ReturnsAllCultures()
        {
            // Arrange
            var traceId = Guid.NewGuid();
            _baseOptions.IncludedCultures = null; // Null included cultures

            var cultures = new List<ClientCultureResponse>
            {
                new() { CultureCode = "en-US", ClientId = 1006, IsPrimary = true },
                new() { CultureCode = "sv-SE", ClientId = 1006, IsPrimary = false },
                new() { CultureCode = "nb-NO", ClientId = 1006, IsPrimary = false }
            };

            var culturesJson = JsonSerializer.Serialize(cultures);
            _queryClientMock.Setup(x => x.GetAsync(Endpoints.Query.Application.ClientCultures, It.IsAny<string>()))
                .ReturnsAsync(culturesJson);

            // Act
            var result = await _service.GetCultureConfigurations(traceId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(3, result.Count); // All cultures included
            Assert.Contains(result, c => c.CultureCode == "en-US");
            Assert.Contains(result, c => c.CultureCode == "sv-SE");
            Assert.Contains(result, c => c.CultureCode == "nb-NO");
        }
    }
}
