using FeedService.Domain.DTOs.External.DataFeedWatch;
using FeedService.Domain.Models;
using FeedService.Helpers;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FeedServiceTests.Helpers
{
    public class ProductFeedUnifierHelperTests
    {
        private readonly Mock<ILogger> _loggerMock = new();
        private readonly Guid _testTraceId = Guid.NewGuid();

        [Fact]
        public void CombineMarketProducts_WithValidData_ReturnsCombinedProducts()
        {
            // Arrange
            var salesAreaConfigs = CreateSampleSalesAreaConfigurations();
            var cultureConfigs = CreateSampleCultureConfigurations();

            // Act
            var result = ProductFeedUnifierHelper.CombineMarketProducts(
                salesAreaConfigs,
                cultureConfigs,
                _loggerMock.Object,
                _testTraceId);

            // Assert
            Assert.NotNull(result);
            Assert.NotEmpty(result);

            // Verify product contains both culture and sales area specific properties
            var product = result.First();
            Assert.Contains("Title_en-US", product.Keys);
            Assert.Contains("Price_SE", product.Keys);

            // Verify logging
            VerifyLogging(
                LogLevel.Information,
                "Starting product combination process",
                Times.Once());

            VerifyLogging(
                LogLevel.Information,
                "Successfully combined products",
                Times.Once());
        }

        [Fact]
        public void CombineMarketProducts_WithNullSalesAreaConfigs_ThrowsArgumentNullException()
        {
            // Arrange
            List<SalesAreaConfiguration>? nullSalesAreaConfigs = null;
            var cultureConfigs = CreateSampleCultureConfigurations();

            // Act & Assert
            var exception = Assert.Throws<ArgumentNullException>(() =>
                ProductFeedUnifierHelper.CombineMarketProducts(
                    nullSalesAreaConfigs!,
                    cultureConfigs,
                    _loggerMock.Object,
                    _testTraceId));

            Assert.Equal("salesAreaConfigurations", exception.ParamName);

            // Verify error logging
            VerifyErrorLogging(Times.Once());
        }

        [Fact]
        public void CombineMarketProducts_WithNullCultureConfigs_ThrowsArgumentNullException()
        {
            // Arrange
            var salesAreaConfigs = CreateSampleSalesAreaConfigurations();
            List<CultureConfiguration>? nullCultureConfigs = null;

            // Act & Assert
            var exception = Assert.Throws<ArgumentNullException>(() =>
                ProductFeedUnifierHelper.CombineMarketProducts(
                    salesAreaConfigs,
                    nullCultureConfigs!,
                    _loggerMock.Object,
                    _testTraceId));

            Assert.Equal("cultureConfigurations", exception.ParamName);

            // Verify error logging
            VerifyErrorLogging(Times.Once());
        }

        [Fact]
        public void CombineMarketProducts_WithEmptyConfigs_ThrowsArgumentException()
        {
            // Arrange
            var emptySalesAreaConfigs = new List<SalesAreaConfiguration>();
            var emptyCultureConfigs = new List<CultureConfiguration>();

            // Assert
            // Act & Assert
            Assert.Throws<ArgumentException>(() =>
                    ProductFeedUnifierHelper.CombineMarketProducts(
                    emptySalesAreaConfigs,
                    emptyCultureConfigs!,
                    _loggerMock.Object,
                    _testTraceId));
        }

        [Fact]
        public void ProcessCultureProducts_WithValidProducts_AddsCultureSpecificProperties()
        {
            // Arrange
            var productsByPartNo = new Dictionary<string, Dictionary<string, object>>();
            var culture = CreateSampleCultureConfigurations().First();

            // Act
            ProductFeedUnifierHelper.ProcessCultureProducts(culture, productsByPartNo);

            // Assert
            Assert.NotEmpty(productsByPartNo);
            var product = productsByPartNo.First();

            // Check key exists
            Assert.Contains($"Title_en-US", product.Value.Keys);

            // Verify value is correct
            Assert.Equal("Test Product US", product.Value["Title_en-US"]);
            Assert.Equal("Test Description US", product.Value["Description_en-US"]);
            Assert.Equal("http://test.com/us/product/test", product.Value["ProductLink_en-US"]);
            Assert.Equal("120.00", product.Value["Price_en-US"]);
            Assert.Equal("USD", product.Value["Currency_en-US"]);
        }

        [Fact]
        public void ProcessSalesAreaProducts_WithValidProducts_AddsSalesAreaSpecificProperties()
        {
            // Arrange
            var productsByPartNo = new Dictionary<string, Dictionary<string, object>>();
            var salesArea = CreateSampleSalesAreaConfigurations().First();

            // Act
            ProductFeedUnifierHelper.ProcessSalesAreaProducts(salesArea, productsByPartNo);

            // Assert
            Assert.NotEmpty(productsByPartNo);

            var product = productsByPartNo.First();
            Assert.Equal("TEST001", product.Key); // Verify the product key is correct

            // Check that all expected keys exist with correct values
            Assert.Equal(1, product.Value[$"Id_{salesArea.SalesAreaCode}"]);
            Assert.Equal("Test Product Swedish", product.Value[$"Name_{salesArea.SalesAreaCode}"]);
            Assert.Equal(100.00m, product.Value[$"Price_{salesArea.SalesAreaCode}"]);
            Assert.Equal(125.00m, product.Value[$"PriceIncVat_{salesArea.SalesAreaCode}"]);
            Assert.Equal(150.00m, product.Value[$"PriceRecommended_{salesArea.SalesAreaCode}"]);
            Assert.Equal(1.25m, product.Value[$"VatRate_{salesArea.SalesAreaCode}"]);
            Assert.Equal(50m, product.Value[$"Stock_{salesArea.SalesAreaCode}"]);
            Assert.Equal(25m, product.Value[$"IncomingStock_{salesArea.SalesAreaCode}"]);
            Assert.Equal(3, product.Value[$"LeadTimeDayCount_{salesArea.SalesAreaCode}"]);
            Assert.Equal(1m, product.Value[$"RecommendedQuantity_{salesArea.SalesAreaCode}"]);

            Assert.True((bool)product.Value[$"IsStockActive_{salesArea.SalesAreaCode}"]);
            Assert.True((bool)product.Value[$"IsReturnable_{salesArea.SalesAreaCode}"]);
            Assert.True((bool)product.Value[$"IsBuyable_{salesArea.SalesAreaCode}"]);
            Assert.False((bool)product.Value[$"IsRecommendedQuantityFixed_{salesArea.SalesAreaCode}"]);

            // Verify date properties exist (exact values will vary)
            Assert.NotNull(product.Value[$"NextDeliveryDate_{salesArea.SalesAreaCode}"]);
            Assert.NotNull(product.Value[$"LastChecked_{salesArea.SalesAreaCode}"]);
            Assert.NotNull(product.Value[$"Updated_{salesArea.SalesAreaCode}"]);
        }

        [Fact]
        public void AddPropertiesWithSuffix_WithValidData_AddsPropertiesCorrectly()
        {
            // Arrange
            var source = new DataFeedWatchDto
            {
                Id = "TEST001",
                Title = "Test Product",
                Price = "100.00"
            };
            var destination = new Dictionary<string, object>();
            var suffix = "US";

            // Act
            ProductFeedUnifierHelper.AddPropertiesWithSuffix(source, destination, suffix);

            // Assert
            Assert.Contains("Id_US", destination.Keys);
            Assert.Contains("Title_US", destination.Keys);
            Assert.Contains("Price_US", destination.Keys);
            Assert.Equal("TEST001", destination["Id_US"]);
            Assert.Equal("Test Product", destination["Title_US"]);
            Assert.Equal("100.00", destination["Price_US"]);
        }

        #region Test Helpers

        private List<SalesAreaConfiguration> CreateSampleSalesAreaConfigurations()
        {
            return new List<SalesAreaConfiguration>
        {
            new()
            {
                SalesAreaId = 1,
                SalesAreaCode = "SE",
                CurrencyCode = "SEK",
                PriceListIds = new List<int> { 1, 2 },
                IsPrimary = true,
                Created = DateTime.UtcNow,
                Updated = null,
                ProductsPriceInfo = new List<PriceProduct>
                {
                    new()
                    {
                        Id = 1,
                        PartNo = "TEST001",
                        Name = "Test Product Swedish",
                        Price = 100.00m,
                        PriceIncVat = 125.00m,
                        PriceRecommended = 150.00m,
                        PriceCatalog = null,
                        VatRate = 1.25m,
                        Stock = 50,
                        IncomingStock = 25,
                        NextDeliveryDate = DateTime.UtcNow.AddDays(7),
                        LeadTimeDayCount = 3,
                        IsStockActive = true,
                        LastChecked = DateTime.UtcNow,
                        IsReturnable = true,
                        RecommendedQuantity = 1,
                        IsRecommendedQuantityFixed = false,
                        IsBuyable = true,
                        Updated = DateTime.UtcNow
                    }
                }
            },
            new()
            {
                SalesAreaId = 2,
                SalesAreaCode = "NO",
                CurrencyCode = "NOK",
                PriceListIds = new List<int> { 3, 4 },
                IsPrimary = false,
                Created = DateTime.UtcNow,
                Updated = DateTime.UtcNow,
                ProductsPriceInfo = new List<PriceProduct>
                {
                    new()
                    {
                        Id = 2,
                        PartNo = "TEST001",
                        Name = "Test Product Norwegian",
                        Price = 120.00m,
                        PriceIncVat = 150.00m,
                        PriceRecommended = 180.00m,
                        PriceCatalog = 200.00m,
                        VatRate = 1.25m,
                        Stock = 30,
                        IncomingStock = 15,
                        NextDeliveryDate = DateTime.UtcNow.AddDays(5),
                        LeadTimeDayCount = 2,
                        IsStockActive = true,
                        LastChecked = DateTime.UtcNow,
                        IsReturnable = true,
                        RecommendedQuantity = 1,
                        IsRecommendedQuantityFixed = false,
                        IsBuyable = true,
                        Updated = DateTime.UtcNow
                    }
                }
            }
        };
        }

        private List<CultureConfiguration> CreateSampleCultureConfigurations()
        {
            return new List<CultureConfiguration>
        {
            new()
            {
                MarketCode = "US",
                CultureCode = "en-US",
                Products = new List<DataFeedWatchDto>
                {
                    new()
                    {
                        Id = "TEST001",
                        Title = "Test Product US",
                        Description = "Test Description US",
                        ProductLink = "http://test.com/us/product/test",
                        ImageLink = "http://test.com/images/test.jpg",
                        Price = "120.00",
                        Currency = "USD",
                        Availability = "in_stock",
                        Category = "Test Category",
                        EanCode = "1234567890123",
                        VariantFlags = new Dictionary<string, string>
                        {
                            { "flag1", "value1" }
                        },
                        Parametrics = new Dictionary<string, string>
                        {
                            { "param1", "value1" }
                        }
                    }
                }
            },
            new()
            {
                MarketCode = "GB",
                CultureCode = "en-GB",
                Products = new List<DataFeedWatchDto>
                {
                    new()
                    {
                        Id = "TEST001",
                        Title = "Test Product GB",
                        Description = "Test Description GB",
                        ProductLink = "http://test.com/gb/product/test",
                        ImageLink = "http://test.com/images/test.jpg",
                        Price = "100.00",
                        Currency = "GBP",
                        Availability = "in_stock",
                        Category = "Test Category",
                        EanCode = "1234567890123",
                        VariantFlags = new Dictionary<string, string>
                        {
                            { "flag1", "value1" }
                        },
                        Parametrics = new Dictionary<string, string>
                        {
                            { "param1", "value1" }
                        }
                    }
                }
            }
        };
        }

        #endregion

        #region Helper Methods for Logging Verification

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
