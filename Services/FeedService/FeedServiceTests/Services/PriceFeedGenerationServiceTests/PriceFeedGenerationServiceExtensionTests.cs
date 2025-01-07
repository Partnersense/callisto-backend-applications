using FeedService.Services.PriceFeedGenerationServices;
using Moq;
using SharedLib.Models.Norce.Api;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace FeedServiceTests.Services.PriceFeedGenerationServiceTests
{
    public class PriceFeedGenerationServiceExtensionTests
    {
        private readonly Mock<ILogger> _loggerMock;
        private readonly Guid _testTraceId;

        public PriceFeedGenerationServiceExtensionTests()
        {
            _loggerMock = new Mock<ILogger>();
            _testTraceId = Guid.NewGuid();
        }

        #region MapListProductsToPriceProducts Tests

        [Fact]
        public void MapListProductsToPriceProducts_WithValidProducts_ReturnsCorrectlyMappedProducts()
        {
            // Arrange
            var products = new List<ListProducts2Response>
        {
            new()
            {
                Id = 1,
                PartNo = "TEST-001",
                Name = "Test Product 1",
                Price = 100.00m,
                PriceIncVat = 125.00m,
                PriceRecommended = 150.00m,
                PriceCatalog = 120.00m,
                VatRate = 1.25m,
                OnHand = new StockInfo
                {
                    Value = 10,
                    IncomingValue = 5,
                    NextDeliveryDate = DateTime.UtcNow.AddDays(1),
                    LeadtimeDayCount = 2,
                    IsActive = true,
                    LastChecked = DateTime.UtcNow,
                    IsReturnable = true
                },
                RecommendedQuantity = 1,
                IsRecommendedQuantityFixed = false,
                IsBuyable = true,
                Updated = DateTime.UtcNow
            }
        };

            // Act
            var result = PriceFeedGenerationServiceExtension.MapListProductsToPriceProducts(products, _loggerMock.Object, _testTraceId);

            // Assert
            Assert.NotNull(result);
            Assert.Single(result);

            var mappedProduct = result[0];
            var sourceProduct = products[0];

            Assert.Equal(sourceProduct.Id, mappedProduct.Id);
            Assert.Equal(sourceProduct.PartNo, mappedProduct.PartNo);
            Assert.Equal(sourceProduct.Name, mappedProduct.Name);
            Assert.Equal(sourceProduct.Price, mappedProduct.Price);
            Assert.Equal(sourceProduct.PriceIncVat, mappedProduct.PriceIncVat);
            Assert.Equal(sourceProduct.PriceRecommended, mappedProduct.PriceRecommended);
            Assert.Equal(sourceProduct.PriceCatalog, mappedProduct.PriceCatalog);
            Assert.Equal(sourceProduct.VatRate, mappedProduct.VatRate);
            Assert.Equal(sourceProduct.OnHand.Value, mappedProduct.Stock);
            Assert.Equal(sourceProduct.OnHand.IncomingValue, mappedProduct.IncomingStock);
            Assert.Equal(sourceProduct.OnHand.NextDeliveryDate, mappedProduct.NextDeliveryDate);
            Assert.Equal(sourceProduct.OnHand.LeadtimeDayCount, mappedProduct.LeadTimeDayCount);
            Assert.Equal(sourceProduct.OnHand.IsActive, mappedProduct.IsStockActive);
            Assert.Equal(sourceProduct.OnHand.LastChecked, mappedProduct.LastChecked);
            Assert.Equal(sourceProduct.OnHand.IsReturnable, mappedProduct.IsReturnable);
            Assert.Equal(sourceProduct.RecommendedQuantity, mappedProduct.RecommendedQuantity);
            Assert.Equal(sourceProduct.IsRecommendedQuantityFixed, mappedProduct.IsRecommendedQuantityFixed);
            Assert.Equal(sourceProduct.IsBuyable, mappedProduct.IsBuyable);
            Assert.Equal(sourceProduct.Updated, mappedProduct.Updated);
        }

        [Fact]
        public void MapListProductsToPriceProducts_WithNullProducts_ThrowsArgumentNullException()
        {
            // Arrange
            List<ListProducts2Response>? nullProducts = null;

            // Act & Assert
            var exception = Assert.Throws<ArgumentNullException>(() =>
                PriceFeedGenerationServiceExtension.MapListProductsToPriceProducts(
                    nullProducts!,
                    _loggerMock.Object,
                    _testTraceId));

            Assert.Equal("products", exception.ParamName);

            // Verify error logging
            VerifyErrorLogging(Times.Once());
        }

        [Fact]
        public void MapListProductsToPriceProducts_WithInvalidProducts_SkipsInvalidAndReturnsValidProducts()
        {
            // Arrange
            var products = new List<ListProducts2Response>
        {
            new() // Invalid product (missing PartNo)
            {
                Id = 1,
                Name = "Invalid Product",
                Price = 100.00m,
                PriceIncVat = 125.00m,
                VatRate = 1.25m,
                IsBuyable = true
            },
            new() // Valid product
            {
                Id = 2,
                PartNo = "TEST-002",
                Name = "Valid Product",
                Price = 100.00m,
                PriceIncVat = 125.00m,
                VatRate = 1.25m,
                IsBuyable = true
            }
        };

            // Act
            var result = PriceFeedGenerationServiceExtension.MapListProductsToPriceProducts(
                products,
                _loggerMock.Object,
                _testTraceId);

            // Assert
            Assert.NotNull(result);
            Assert.Single(result);
            Assert.Equal("TEST-002", result[0].PartNo);

            // Verify validation logging
            VerifyLogging(
                LogLevel.Information,
                "Product validation failed",
                Times.Once());
        }

        [Fact]
        public void MapListProductsToPriceProducts_WithEmptyList_ReturnsEmptyList()
        {
            // Arrange
            var emptyProducts = new List<ListProducts2Response>();

            // Act
            var result = PriceFeedGenerationServiceExtension.MapListProductsToPriceProducts(
                emptyProducts,
                _loggerMock.Object,
                _testTraceId);

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
        }

        #endregion

        #region IsProductValid Tests

        [Theory]
        [InlineData(null)] // Missing PartNo
        [InlineData("")] // Empty PartNo
        [InlineData(" ")] // Whitespace PartNo
        public void IsProductValid_WithInvalidPartNo_ReturnsFalseAndLogs(string? partNo)
        {
            // Arrange
            var product = new ListProducts2Response
            {
                Id = 1,
                PartNo = partNo,
                Price = 100.00m,
                PriceIncVat = 125.00m,
                VatRate = 1.25m
            };

            // Act
            var result = PriceFeedGenerationServiceExtension.IsProductValid(
                product,
                _loggerMock.Object,
                _testTraceId);

            // Assert
            Assert.False(result);
            VerifyLogging(LogLevel.Information, "Missing PartNo", Times.Once());
        }

        [Theory]
        [InlineData(-1.00)] // Negative price
        [InlineData(-0.01)] // Small negative price
        public void IsProductValid_WithNegativePrice_ReturnsFalseAndLogs(decimal price)
        {
            // Arrange
            var product = new ListProducts2Response
            {
                Id = 1,
                PartNo = "TEST-001",
                Price = price,
                PriceIncVat = 125.00m,
                VatRate = 1.25m
            };

            // Act
            var result = PriceFeedGenerationServiceExtension.IsProductValid(
                product,
                _loggerMock.Object,
                _testTraceId);

            // Assert
            Assert.False(result);
            VerifyLogging(LogLevel.Information, "Negative Price", Times.Once());
        }

        [Theory]
        [InlineData(-1.00)] // Negative price inc VAT
        [InlineData(-0.01)] // Small negative price inc VAT
        public void IsProductValid_WithNegativePriceIncVat_ReturnsFalseAndLogs(decimal priceIncVat)
        {
            // Arrange
            var product = new ListProducts2Response
            {
                Id = 1,
                PartNo = "TEST-001",
                Price = 100.00m,
                PriceIncVat = priceIncVat,
                VatRate = 1.25m
            };

            // Act
            var result = PriceFeedGenerationServiceExtension.IsProductValid(
                product,
                _loggerMock.Object,
                _testTraceId);

            // Assert
            Assert.False(result);
            VerifyLogging(
                LogLevel.Information,
                "Negative PriceIncVat",
                Times.Once());
        }

        [Theory]
        [InlineData(-1.25)] // Negative VAT rate
        public void IsProductValid_WithInvalidVatRate_ReturnsFalseAndLogs(decimal vatRate)
        {
            // Arrange
            var product = new ListProducts2Response
            {
                Id = 1,
                PartNo = "TEST-001",
                Price = 100.00m,
                PriceIncVat = 125.00m,
                VatRate = vatRate
            };

            // Act
            var result = PriceFeedGenerationServiceExtension.IsProductValid(
                product,
                _loggerMock.Object,
                _testTraceId);

            // Assert
            Assert.False(result);
            VerifyLogging(
                LogLevel.Information,
                "Invalid VatRate",
                Times.Once());
        }

        [Theory]
        [InlineData(0)] // Zero VAT rate
        public void IsProductValid_WithZeroVat_ReturnTrue(decimal vatRate)
        {
            // Arrange
            var product = new ListProducts2Response
            {
                Id = 1,
                PartNo = "TEST-001",
                Price = 100.00m,
                PriceIncVat = 125.00m,
                VatRate = vatRate
            };

            // Act
            var result = PriceFeedGenerationServiceExtension.IsProductValid(
                product,
                _loggerMock.Object,
                _testTraceId);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void IsProductValid_WithMultipleValidationErrors_LogsAllErrors()
        {
            // Arrange
            var product = new ListProducts2Response
            {
                Id = 1,
                PartNo = "", // Invalid
                Price = -100.00m, // Invalid
                PriceIncVat = -125.00m, // Invalid
                VatRate = -1 // Invalid
            };

            // Act
            var result = PriceFeedGenerationServiceExtension.IsProductValid(
                product,
                _loggerMock.Object,
                _testTraceId);

            // Assert
            Assert.False(result);

            // Verify that all validation errors were logged
            VerifyLogging(
                LogLevel.Information,
                "Missing PartNo, Negative Price, Negative PriceIncVat, Invalid VatRate",
                Times.Once());
        }

        [Fact]
        public void IsProductValid_WithValidProduct_ReturnsTrue()
        {
            // Arrange
            var product = new ListProducts2Response
            {
                Id = 1,
                PartNo = "TEST-001",
                Price = 100.00m,
                PriceIncVat = 125.00m,
                VatRate = 1.25m
            };

            // Act
            var result = PriceFeedGenerationServiceExtension.IsProductValid(
                product,
                _loggerMock.Object,
                _testTraceId);

            // Assert
            Assert.True(result);

            // Verify no validation errors were logged
            VerifyLogging(
                LogLevel.Information,
                "Product validation failed",
                Times.Never());
        }

        #endregion

        #region Helper Methods

        private void VerifyLogging(LogLevel logLevel, string messageContains, Times times)
        {
            _loggerMock.Verify(
                x => x.Log(
                    logLevel,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains(messageContains)),
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
