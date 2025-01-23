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
                IsActive = true,
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
        public void MapSalesAreaToSalesAreaConfiguration_WithPriceListWithNullSalesAre_PrimarySalesArea_IncludePriceList()
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
                    SalesAreaId = null,
                    CurrencyId = 1,
                    IsActive = true,
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
        public void MapSalesAreaToSalesAreaConfiguration_WithPriceListWithNullSalesAre_NotPrimarySalesArea_DoesNotIncludePriceList()
        {
            // Arrange
            var salesArea = new SalesAreaResponse
            {
                SalesAreaId = 1,
                Code = "US",
                IsPrimary = false,
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
                    SalesAreaId = null,
                    CurrencyId = 1,
                    IsActive = true,
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
            Assert.Null(result);
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
                IsActive = true,
                PriceList = new PriceListResponse { Id = 1, IsActive = true }
            },
            new()
            {
                PriceListId = 2,
                SalesAreaId = 1,
                CurrencyId = 1,
                IsActive = true,
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
                IsActive = true,
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

        [Fact]
        public void FilterSalesAreasByIncludedIds_WithMatchingIds_ReturnsFilteredList()
        {
            // Arrange
            var salesAreas = new List<SalesAreaResponse>
            {
                new() { SalesAreaId = 1, Code = "US" },
                new() { SalesAreaId = 2, Code = "SE" },
                new() { SalesAreaId = 3, Code = "NO" }
            };

            var includedIds = new List<int> { 1, 3 };

            // Act
            var result = SalesAreaConfigurationServiceExtension.FilterSalesAreasByIncludedIds(
                salesAreas, includedIds, _loggerMock.Object, _testTraceId);

            // Assert
            Assert.Equal(2, result.Count);
            Assert.Contains(result, sa => sa.SalesAreaId == 1);
            Assert.Contains(result, sa => sa.SalesAreaId == 3);
            Assert.DoesNotContain(result, sa => sa.SalesAreaId == 2);
        }

        [Fact]
        public void FilterSalesAreasByIncludedIds_WithEmptyIncludedIds_ReturnsAllSalesAreas()
        {
            // Arrange
            var salesAreas = new List<SalesAreaResponse>
            {
                new() { SalesAreaId = 1, Code = "US" },
                new() { SalesAreaId = 2, Code = "SE" }
            };

            var emptyIncludedIds = new List<int>();

            // Act
            var result = SalesAreaConfigurationServiceExtension.FilterSalesAreasByIncludedIds(
                salesAreas, emptyIncludedIds, _loggerMock.Object, _testTraceId);

            // Assert
            Assert.Equal(salesAreas.Count, result.Count);
            Assert.Equal(salesAreas, result);
        }

        [Fact]
        public void FilterSalesAreasByIncludedIds_WithNullIncludedIds_ReturnsAllSalesAreas()
        {
            // Arrange
            var salesAreas = new List<SalesAreaResponse>
            {
                new() { SalesAreaId = 1, Code = "US" },
                new() { SalesAreaId = 2, Code = "SE" }
            };

            // Act
            var result = SalesAreaConfigurationServiceExtension.FilterSalesAreasByIncludedIds(
                salesAreas, null, _loggerMock.Object, _testTraceId);

            // Assert
            Assert.Equal(salesAreas.Count, result.Count);
            Assert.Equal(salesAreas, result);
        }

        [Fact]
        public void FilterSalesAreasByIncludedIds_WithNullSalesAreas_ThrowsArgumentNullException()
        {
            // Arrange
            List<SalesAreaResponse> nullSalesAreas = null;
            var includedIds = new List<int> { 1, 2 };

            // Act & Assert
            var exception = Assert.Throws<ArgumentNullException>(() =>
                SalesAreaConfigurationServiceExtension.FilterSalesAreasByIncludedIds(
                    nullSalesAreas, includedIds, _loggerMock.Object, _testTraceId));

            Assert.Equal("salesAreas", exception.ParamName);
        }

        [Fact]
        public void FilterSalesAreasByIncludedIds_WithNoMatchingIds_ReturnsEmptyList()
        {
            // Arrange
            var salesAreas = new List<SalesAreaResponse>
            {
                new() { SalesAreaId = 1, Code = "US" },
                new() { SalesAreaId = 2, Code = "SE" }
            };

            var nonMatchingIds = new List<int> { 3, 4 };

            // Act
            var result = SalesAreaConfigurationServiceExtension.FilterSalesAreasByIncludedIds(
                salesAreas, nonMatchingIds, _loggerMock.Object, _testTraceId);

            // Assert
            Assert.Empty(result);
        }

        [Fact]
        public void FilterSalesAreasByIncludedIds_VerifyLoggingOnSuccess()
        {
            // Arrange
            var salesAreas = new List<SalesAreaResponse>
            {
                new() { SalesAreaId = 1, Code = "US" },
                new() { SalesAreaId = 2, Code = "SE" }
            };

            var includedIds = new List<int> { 1 };

            // Act
            var result = SalesAreaConfigurationServiceExtension.FilterSalesAreasByIncludedIds(
                salesAreas, includedIds, _loggerMock.Object, _testTraceId);

            // Assert
            _loggerMock.Verify(
                x => x.Log(
                    LogLevel.Information,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Successfully filtered sales areas")),
                    null,
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.Once);
        }
    }
}
