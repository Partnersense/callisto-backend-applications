using Enferno.Services.StormConnect.Contracts.Product;
using Moq;
using Serilog;
using SharedLib.Constants;
using SharedLib.Constants.NorceConstants;
using SharedLib.Mappers;
using SharedLib.Models.Sage;

namespace SharedLibTests.Mappers;

public class SageMapperTests
{
    public SageMapperTests()
    {
        Log.Logger = new LoggerConfiguration().CreateLogger();
    }

    [Fact]
    public void MapSageProductToNorceProduct_ShouldMapFieldsCorrectly()
    {
        // Arrange
        var sageQuery = new List<QueryLine>
            {
                new()
                {
                    Fields = new List<Field>
                    {
                        new() { Name = SageConstants.Fields.ProductField.StatusCodes, Value = "status1;status2;status3;status4;B" },
                        new() { Name = SageConstants.Fields.ProductField.DescriptionEn, Value = "Test Product" },
                        new() { Name = SageConstants.Fields.ProductField.PartNo, Value = "PART123" },
                        new() { Name = SageConstants.Fields.ProductField.CSVdata, Value = "artistSignature;limitedEdition" },
                        new() { Name = SageConstants.Fields.ProductField.SupplierName, Value = "Supplier name"},
                    }
                }
            };

        // Act
        var result = SageMapper.MapSageProductToNorceProduct("PART", sageQuery);
        var supplierParametric = result?.ProductParametrics.Find(parametric =>
            parametric.ParametricCode == NorceConstants.Parametrics.Supplier);
        // Assert
        Assert.NotNull(result);

        // Commented out because this is temporarily hard coded in the mapper
        //Assert.Equal("MFG123", result.ManufacturerCode);
        //Assert.Equal("Manufacturer Name", result.ManufacturerName);

        Assert.Contains("Supplier name", supplierParametric?.ValueList.Value);
        //Assert.Equal("PART", result.Code); //Code is removed to not group products together
        Assert.Single(result.Variants);

        var variant = result.Variants[0];
        Assert.Equal("PART123", variant.ManufacturerPartNo);
        Assert.Single(variant.Cultures);
        Assert.Equal(LanguageConstants.CultureCode.USA, variant.Cultures[0].CultureCode);
        Assert.Equal("Test Product", variant.Cultures[0].Name);
        Assert.True(variant.VariantFlags.Count >= 3);
        Assert.True(variant.VariantFlags[0].IsSet);
        Assert.Equal(NorceConstants.Flags.ProductDataSpecialistStatus.VariantFlags.ReadyForSetup, variant.VariantFlags[0].Code);
        Assert.True(variant.VariantFlags[1].IsSet);
        Assert.Equal(NorceConstants.Flags.EcommerceManagerStatus.VariantFlags.ReadyForSetup, variant.VariantFlags[1].Code);
        Assert.True(variant.VariantFlags[2].IsSet);
        Assert.Equal(NorceConstants.Flags.MediaStatus.VariantFlags.ReadyForSetup, variant.VariantFlags[2].Code);

        Assert.Single(variant.Skus);
        var sku = variant.Skus[0];
        Assert.Equal("PART123", sku.PartNo);
        Assert.Single(sku.Cultures);
        Assert.Equal(LanguageConstants.CultureCode.USA, sku.Cultures.ToList()[0].CultureCode);

        Assert.Equal(2, variant.VariantParametrics.Count);
        Assert.Equal(NorceConstants.Parametrics.ArtistSignatureProductCode, variant.VariantParametrics[0].ParametricCode);
        Assert.Equal("artistsignature", variant.VariantParametrics[0].ValueList.Code);
        Assert.Equal(NorceConstants.Parametrics.LimitedEditionCode, variant.VariantParametrics[1].ParametricCode);
        Assert.Equal("limitededition", variant.VariantParametrics[1].ValueList.Code);

        Assert.True(result.ProductFlags.Count >= 3);
        Assert.True(result.ProductFlags[0].IsSet);
        Assert.Equal(NorceConstants.Flags.ProductDataSpecialistStatus.ProductFlags.ReadyForSetup, result.ProductFlags[0].Code);
        Assert.True(result.ProductFlags[1].IsSet);
        Assert.Equal(NorceConstants.Flags.EcommerceManagerStatus.ProductFlags.ReadyForSetup, result.ProductFlags[1].Code);
        Assert.True(result.ProductFlags[2].IsSet);
        Assert.Equal(NorceConstants.Flags.MediaStatus.ProductFlags.ReadyForSetup, result.ProductFlags[2].Code);
    }

    [Fact]
    public void MapSageStockToNorceOnhands_ShouldMapFieldsCorrectly()
    {
        // Arrange
        var sageStock = new SageStockResultLine
        {
            Fields = new List<SageStockResultField>
                {
                    new() { Name = SageConstants.Fields.StockField.PartNo, Value = "PART123" },
                    new() { Name = SageConstants.Fields.StockField.Quantity, Value = "10" }
                }
        };

        // Act
        var result = SageMapper.MapSageStockToNorceOnhands(sageStock, WareHouseConstants.WareHouseCodes.Eu);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("PART123", result.PartNo);
        Assert.Equal(WareHouseConstants.Locations.EuropeStandard, result.LocationCode);
        Assert.Equal(WareHouseConstants.WareHouseCodes.Eu, result.WarehouseCode);
        Assert.Equal(10m, result.OnhandValue);
    }

    [Fact]
    public void MapSageStockToNorceOnhands_ShouldReturnNullAndLogWarning_WhenPartNoIsMissing()
    {
        // Arrange
        var mockLogger = new Mock<ILogger>();
        Log.Logger = mockLogger.Object;

        var sageStock = new SageStockResultLine
        {
            Fields = new List<SageStockResultField>
                {
                    new() { Name = SageConstants.Fields.StockField.Quantity, Value = "10" }
                }
        };

        // Act
        var result = SageMapper.MapSageStockToNorceOnhands(sageStock, WareHouseConstants.WareHouseCodes.Eu);

        // Assert
        Assert.Null(result);
        mockLogger.Verify(logger => logger.Warning(It.Is<string>(s => s.Contains("Missing fields: PartNo"))), Times.Once);
    }

    [Fact]
    public void MapSageStockToNorceOnhands_ShouldReturnNullAndLogWarning_WhenOnhandValueIsInvalid()
    {
        // Arrange
        var mockLogger = new Mock<ILogger>();
        Log.Logger = mockLogger.Object;

        var sageStock = new SageStockResultLine
        {
            Fields = new List<SageStockResultField>
                {
                    new() { Name = SageConstants.Fields.StockField.PartNo, Value = "PART123" },
                    new() { Name = "QUANTITEYY", Value = "InvalidQuantity" }
                }
        };

        // Act
        var result = SageMapper.MapSageStockToNorceOnhands(sageStock, WareHouseConstants.WareHouseCodes.Eu);

        // Assert
        Assert.Null(result);
        mockLogger.Verify(logger => logger.Warning(It.Is<string>(s => s.Contains("Missing fields: OnhandValue"))), Times.Once);
    }

    [Fact]
    public void MapSagePriceToNorceSkuPriceList_ShouldMapFieldsCorrectly()
    {
        // Arrange
        var sageQuery = new QueryLine
        {
            Fields = new List<Field>
                {
                    new() { Name = SageConstants.Fields.PriceField.PartNo, Value = "PART123" },
                    new() { Name = SageConstants.Fields.PriceField.Price, Value = "99.99" },
                    new() { Name = SageConstants.Fields.PriceField.CurrencyCode, Value = "USD" }
                }
        };

        // Act
        var result = SageMapper.MapSagePriceToNorceSkuPriceList(sageQuery);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("PART123", result.PartNo);
        Assert.Equal("USD", result.CurrencyCode);
        Assert.Equal(99.99m, result.PriceSale);
        Assert.Equal("usStandard", result.PriceListCode);
    }

    [Fact]
    public void MapSagePriceToNorceSkuPriceList_ShouldReturnNull_WhenPartNoIsMissing()
    {
        // Arrange
        var sageQuery = new QueryLine
        {
            Fields = new List<Field>
                {
                    new() { Name = SageConstants.Fields.PriceField.Price, Value = "99.99" },
                    new() { Name = SageConstants.Fields.PriceField.CurrencyCode, Value = "USD" }
                }
        };

        // Act
        var result = SageMapper.MapSagePriceToNorceSkuPriceList(sageQuery);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public void MapSagePriceToNorceSkuPriceList_ShouldReturnNull_WhenPriceIsInvalid()
    {
        // Arrange
        var sageQuery = new QueryLine
        {
            Fields = new List<Field>
                {
                    new() { Name = SageConstants.Fields.PriceField.PartNo, Value = "PART123" },
                    new() { Name = SageConstants.Fields.PriceField.Price, Value = "InvalidPrice" },
                    new() { Name = SageConstants.Fields.PriceField.CurrencyCode, Value = "USD" }
                }
        };

        // Act
        var result = SageMapper.MapSagePriceToNorceSkuPriceList(sageQuery);

        // Assert
        Assert.Null(result);
    }

    [Theory]
    [InlineData("ONEPART", "ONEPART")]
    [InlineData("ONEPARTB", "ONEPARTB")]
    [InlineData("TWO-PARTS", "TWO-PARTS")]
    [InlineData("TWO-PARTSB", "TWO-PARTS")]
    [InlineData("THREE-PART-S", "THREE-PART-S")]
    [InlineData("THREE-PART-SB", "THREE-PART-SB")]
    [InlineData("123-456-7-8-90", "123-456-7-8-90")]
    [InlineData("123-456B-7-8-90", "123-456-7-8-90")]
    public void ExtractInstrumentProductionSku_ShouldReturnAStock_WhenBStock(string partNo, string expected)
    {
        // Act
        var result = SageMapper.ExtractInstrumentProductionSku(partNo);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(expected, result.ValueList.Value); 
    }
}
