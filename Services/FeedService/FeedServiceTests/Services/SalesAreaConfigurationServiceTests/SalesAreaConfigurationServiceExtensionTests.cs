using FeedService.Services.SalesAreaConfigurationServices;
using Microsoft.Extensions.Logging;
using Moq;
using SharedLib.Models.Norce.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FeedServiceTests.Services.SalesAreaConfigurationServiceTests
{
    public class SalesAreaConfigurationServiceExtensionTests
    {
        private readonly Mock<ILogger<SalesAreaConfigurationService>> _loggerMock;
        private readonly Guid _testTraceId;

        public SalesAreaConfigurationServiceExtensionTests()
        {
            _loggerMock = new Mock<ILogger<SalesAreaConfigurationService>>();
            _testTraceId = Guid.NewGuid();
        }

        [Fact]
        public void MapSalesAreaToSalesAreaConfiguration_WithValidData_ReturnsSalesAreaConfiguration()
        {
            // Arrange
            var salesArea = new SalesAreaResponse
            {
                SalesAreaId = 1,
                Code = "US",
                IsPrimary = true,
                Created = DateTime.UtcNow,
                Updated = null,
                ClientId = 1006,
                CreatedBy = 1
            };

            var priceLists = new List<PriceListClientResponse>
        {
            new()
            {
                PriceListId = 1,
                SalesAreaId = 1,
                CurrencyId = 1,
                PriceList = new PriceListResponse
                {
                    Id = 1,
                    IsActive = true,
                    DefaultName = "Standard US"
                }
            }
        };

            var currencies = new List<CurrenciesResponse>
        {
            new()
            {
                Id = 1,
                Code = "USD",
                DefaultName = "US Dollar",
                IsActive = true
            }
        };

            // Act
            var result = SalesAreaConfigurationServiceExtension.MapSalesAreaToSalesAreaConfiguration(
                salesArea, priceLists, currencies, _loggerMock.Object, _testTraceId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(salesArea.SalesAreaId, result.SalesAreaId);
            Assert.Equal(salesArea.Code, result.SalesAreaCode);
            Assert.Equal("USD", result.CurrencyCode);
            Assert.Equal(salesArea.IsPrimary, result.IsPrimary);
            Assert.Equal(salesArea.Created, result.Created);
            Assert.Equal(salesArea.Updated, result.Updated);
            Assert.Single(result.PriceListIds);
            Assert.Equal(1, result.PriceListIds[0]);
        }

        [Fact]
        public void MapSalesAreaToSalesAreaConfiguration_WithInactivePriceLists_ReturnsNull()
        {
            // Arrange
            var salesArea = new SalesAreaResponse
            {
                SalesAreaId = 1,
                Code = "US",
                IsPrimary = true,
                Created = DateTime.UtcNow
            };

            var priceLists = new List<PriceListClientResponse>
        {
            new()
            {
                PriceListId = 1,
                SalesAreaId = 1,
                CurrencyId = 1,
                PriceList = new PriceListResponse
                {
                    Id = 1,
                    IsActive = false,
                    DefaultName = "Standard US"
                }
            }
        };

            var currencies = new List<CurrenciesResponse>
        {
            new()
            {
                Id = 1,
                Code = "USD",
                DefaultName = "US Dollar",
                IsActive = true
            }
        };

            // Act
            var result = SalesAreaConfigurationServiceExtension.MapSalesAreaToSalesAreaConfiguration(
                salesArea, priceLists, currencies, _loggerMock.Object, _testTraceId);

            // Assert
            Assert.Null(result);
            _loggerMock.Verify(
                x => x.Log(
                    LogLevel.Information,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("No pricelist for Sales area")),
                    null,
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.Once);
        }

        [Fact]
        public void GetCurrencyCodeForSalesArea_WithValidData_ReturnsCurrencyCode()
        {
            // Arrange
            var salesArea = new SalesAreaResponse { SalesAreaId = 1 };
            var priceLists = new List<PriceListClientResponse>
        {
            new()
            {
                PriceListId = 1,
                SalesAreaId = 1,
                CurrencyId = 1
            }
        };

            var currencies = new List<CurrenciesResponse>
        {
            new()
            {
                Id = 1,
                Code = "USD",
                DefaultName = "US Dollar"
            }
        };

            // Act
            var result = SalesAreaConfigurationServiceExtension.GetCurrencyCodeForSalesArea(
                salesArea, priceLists, currencies, _loggerMock.Object, _testTraceId);

            // Assert
            Assert.Equal("USD", result);
        }

        [Fact]
        public void GetCurrencyCodeForSalesArea_WithInvalidCurrencyId_ThrowsArgumentNullException()
        {
            // Arrange
            var salesArea = new SalesAreaResponse { SalesAreaId = 1 };
            var priceLists = new List<PriceListClientResponse>
        {
            new()
            {
                PriceListId = 1,
                SalesAreaId = 1,
                CurrencyId = 999 // Non-existent currency ID
            }
        };

            var currencies = new List<CurrenciesResponse>
        {
            new()
            {
                Id = 1,
                Code = "USD",
                DefaultName = "US Dollar"
            }
        };

            // Act & Assert
            var exception = Assert.Throws<ArgumentNullException>(() =>
                SalesAreaConfigurationServiceExtension.GetCurrencyCodeForSalesArea(
                    salesArea, priceLists, currencies, _loggerMock.Object, _testTraceId));

            Assert.Equal("currencyInfo", exception.ParamName);
        }

        [Fact]
        public void MapSalesAreaToSalesAreaConfiguration_WithMultiplePriceLists_ReturnsSalesAreaConfigurationWithAllPriceListIds()
        {
            // Arrange
            var salesArea = new SalesAreaResponse
            {
                SalesAreaId = 1,
                Code = "US",
                IsPrimary = true,
                Created = DateTime.UtcNow
            };

            var priceLists = new List<PriceListClientResponse>
        {
            new()
            {
                PriceListId = 1,
                SalesAreaId = 1,
                CurrencyId = 1,
                PriceList = new PriceListResponse { Id = 1, IsActive = true }
            },
            new()
            {
                PriceListId = 2,
                SalesAreaId = 1,
                CurrencyId = 1,
                PriceList = new PriceListResponse { Id = 2, IsActive = true }
            }
        };

            var currencies = new List<CurrenciesResponse>
        {
            new()
            {
                Id = 1,
                Code = "USD",
                DefaultName = "US Dollar",
                IsActive = true
            }
        };

            // Act
            var result = SalesAreaConfigurationServiceExtension.MapSalesAreaToSalesAreaConfiguration(
                salesArea, priceLists, currencies, _loggerMock.Object, _testTraceId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.PriceListIds.Count);
            Assert.Contains(1, result.PriceListIds);
            Assert.Contains(2, result.PriceListIds);
        }

        [Fact]
        public void MapSalesAreaToSalesAreaConfiguration_WithEmptyCurrencies_ThrowsArgumentNullException()
        {
            // Arrange
            var salesArea = new SalesAreaResponse
            {
                SalesAreaId = 1,
                Code = "US",
                IsPrimary = true,
                Created = DateTime.UtcNow
            };

            var priceLists = new List<PriceListClientResponse>
        {
            new()
            {
                PriceListId = 1,
                SalesAreaId = 1,
                CurrencyId = 1,
                PriceList = new PriceListResponse { Id = 1, IsActive = true }
            }
        };

            var currencies = new List<CurrenciesResponse>();

            // Act & Assert
            var exception = Assert.Throws<ArgumentNullException>(() =>
                SalesAreaConfigurationServiceExtension.MapSalesAreaToSalesAreaConfiguration(
                    salesArea, priceLists, currencies, _loggerMock.Object, _testTraceId));

            _loggerMock.Verify(
                x => x.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Value cannot be null. (Parameter 'currencyInfo')")),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.AtLeast(1));
        }
    }
}
