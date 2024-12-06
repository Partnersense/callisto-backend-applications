using FeedService.Services.CultureConfigurationServices;
using Microsoft.Extensions.Logging;
using Moq;
using SharedLib.Models.Norce.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FeedService.Domain.Models;

namespace FeedServiceTests.Services.CultureConfigurationsServicesTests
{
    public class CultureConfigurationServiceExtensionTests
    {
        private readonly Mock<ILogger> _loggerMock = new();
        private readonly Guid _testTraceId = Guid.NewGuid();

        #region MapCultureToMarketConfiguration Tests

        [Fact]
        public void MapCultureToMarketConfiguration_ValidCulture_ReturnsCorrectConfiguration()
        {
            // Arrange
            var culture = new ClientCultureResponse
            {
                CultureCode = "en-US",
                ClientId = 1006,
                IsPrimary = true,
                Created = DateTime.UtcNow,
                CreatedBy = 1
            };

            // Act
            var result = CultureConfigurationServiceExtension.MapCultureToMarketConfiguration(
                culture,
                _loggerMock.Object,
                _testTraceId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("US", result.MarketCode);
            Assert.Equal("en-US", result.CultureCode);

            // Verify success logging
            VerifyLogging(
                LogLevel.Information,
                "Successfully mapped culture to market configuration",
                Times.Once());
        }

        [Fact]
        public void MapCultureToMarketConfiguration_NullCulture_ThrowsInvalidOperationException()
        {
            // Arrange
            ClientCultureResponse? nullCulture = null;

            // Act & Assert
            var exception = Assert.Throws<InvalidOperationException>(() =>
                CultureConfigurationServiceExtension.MapCultureToMarketConfiguration(
                    nullCulture,
                    _loggerMock.Object,
                    _testTraceId));

            Assert.Contains("Invalid culture code format", exception.Message);

            // Verify error logging
            VerifyErrorLogging(Times.Once());
        }

        [Theory]
        [InlineData("invalid")]
        [InlineData("en")]
        [InlineData("en-US-extra")]
        [InlineData("")]
        public void MapCultureToMarketConfiguration_InvalidCultureFormat_ThrowsInvalidOperationException(string invalidCultureCode)
        {
            // Arrange
            var culture = new ClientCultureResponse
            {
                CultureCode = invalidCultureCode,
                ClientId = 1006
            };

            // Act & Assert
            var exception = Assert.Throws<InvalidOperationException>(() =>
                CultureConfigurationServiceExtension.MapCultureToMarketConfiguration(
                    culture,
                    _loggerMock.Object,
                    _testTraceId));

            Assert.Contains("Invalid culture code format", exception.Message);

            // Verify error logging
            VerifyErrorLogging(Times.Once());
        }

        [Theory]
        [InlineData("en-US", "US")]
        [InlineData("sv-SE", "SE")]
        [InlineData("nb-NO", "NO")]
        [InlineData("de-DE", "DE")]
        public void MapCultureToMarketConfiguration_VariousCultures_ExtractsCorrectMarketCode(
            string cultureCode,
            string expectedMarketCode)
        {
            // Arrange
            var culture = new ClientCultureResponse
            {
                CultureCode = cultureCode,
                ClientId = 1006
            };

            // Act
            var result = CultureConfigurationServiceExtension.MapCultureToMarketConfiguration(
                culture,
                _loggerMock.Object,
                _testTraceId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(expectedMarketCode, result.MarketCode);
            Assert.Equal(cultureCode, result.CultureCode);

            // Verify success logging
            VerifyLogging(
                LogLevel.Information,
                "Successfully mapped culture to market configuration",
                Times.Once());
        }

        #endregion

        #region FilterCulturesByIncludedCodes Tests

        [Fact]
        public void FilterCulturesByIncludedCodes_WithNullCultures_ThrowsArgumentNullException()
        {
            // Arrange
            List<ClientCultureResponse>? nullCultures = null;
            var includedCodes = new List<string> { "en-US", "sv-SE" };

            // Act & Assert
            var exception = Assert.Throws<ArgumentNullException>(() =>
                CultureConfigurationServiceExtension.FilterCulturesByIncludedCodes(
                    nullCultures,
                    includedCodes,
                    _loggerMock.Object,
                    _testTraceId));

            Assert.Equal("cultures", exception.ParamName);

            // Verify error logging
            VerifyErrorLogging(Times.Once());
        }

        [Fact]
        public void FilterCulturesByIncludedCodes_WithEmptyIncludedCodes_ReturnsAllCultures()
        {
            // Arrange
            var cultures = new List<ClientCultureResponse>
            {
                new() { CultureCode = "en-US" },
                new() { CultureCode = "sv-SE" }
            };
            var emptyIncludedCodes = new List<string>();

            // Act
            var result = CultureConfigurationServiceExtension.FilterCulturesByIncludedCodes(
                cultures,
                emptyIncludedCodes,
                _loggerMock.Object,
                _testTraceId);

            // Assert
            Assert.Equal(cultures.Count, result.Count);
            Assert.Equal(cultures, result);

            // Verify logging
            VerifyLogging(
                LogLevel.Information,
                "No culture codes filter specified",
                Times.Once());
        }

        [Fact]
        public void FilterCulturesByIncludedCodes_WithValidFilters_ReturnsFilteredCultures()
        {
            // Arrange
            var cultures = new List<ClientCultureResponse>
            {
                new() { CultureCode = "en-US" },
                new() { CultureCode = "sv-SE" },
                new() { CultureCode = "nb-NO" }
            };
            var includedCodes = new List<string> { "en-US", "sv-SE" };

            // Act
            var result = CultureConfigurationServiceExtension.FilterCulturesByIncludedCodes(
                cultures,
                includedCodes,
                _loggerMock.Object,
                _testTraceId);

            // Assert
            Assert.Equal(2, result.Count);
            Assert.Contains(result, c => c.CultureCode == "en-US");
            Assert.Contains(result, c => c.CultureCode == "sv-SE");
            Assert.DoesNotContain(result, c => c.CultureCode == "nb-NO");

            // Verify success logging
            VerifyLogging(
                LogLevel.Information,
                "Successfully filtered cultures",
                Times.Once());
        }

        [Fact]
        public void FilterCulturesByIncludedCodes_CaseInsensitiveMatching()
        {
            // Arrange
            var cultures = new List<ClientCultureResponse>
            {
                new() { CultureCode = "en-US" },
                new() { CultureCode = "sv-SE" }
            };
            var includedCodes = new List<string> { "EN-us", "SV-se" };

            // Act
            var result = CultureConfigurationServiceExtension.FilterCulturesByIncludedCodes(
                cultures,
                includedCodes,
                _loggerMock.Object,
                _testTraceId);

            // Assert
            Assert.Equal(2, result.Count);
            Assert.Contains(result, c => c.CultureCode == "en-US");
            Assert.Contains(result, c => c.CultureCode == "sv-SE");

            // Verify success logging
            VerifyLogging(
                LogLevel.Information,
                "Successfully filtered cultures",
                Times.Once());
        }

        #endregion

        #region Helper Methods

        private void VerifyLogging(LogLevel logLevel, string messageContains, Times times)
        {
            _loggerMock.Verify(
                x => x.Log(
                    logLevel,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString().Contains(messageContains)),
                    null,
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                times);
        }

        private void VerifyErrorLogging(Times times)
        {
            _loggerMock.Verify(
                x => x.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => true),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                times);
        }

        #endregion
    }
}
