using SharedLib.Constants;
using SharedLib.Constants.NorceConstants;

namespace SharedLib.Mappers;

public static class SageToNorceTranslator
{
    public static string SageCodeToNorceParametricCode(string sageCode)
    {
        return sageCode switch
        {
            SageConstants.Fields.ProductField.StatusCode.Boden => NorceConstants.Parametrics.BodyShape.BodenCode,
            SageConstants.Fields.ProductField.StatusCode.Sälen => NorceConstants.Parametrics.BodyShape.SälenCode,
            SageConstants.Fields.ProductField.StatusCode.Varberg => NorceConstants.Parametrics.BodyShape.VarbergCode,
            _ => string.Empty
        };
    }

    public static string SageWarehouseCodeToNorceWarehouseCode(string sageWarehouseCode)
    {
        return sageWarehouseCode switch
        {
            SageConstants.Api.WareHouseCodes.Eu => WareHouseConstants.WareHouseCodes.Eu,
            SageConstants.Api.WareHouseCodes.EuBStock => WareHouseConstants.WareHouseCodes.EuBStock,
            SageConstants.Api.WareHouseCodes.Us => WareHouseConstants.WareHouseCodes.Us,
            _ => string.Empty
        };
    }

    public static (string warehouseCode, string locationCode) SageWarehouseToNorceWarehouse(string sageWarehouseCode)
    {
        return sageWarehouseCode switch
        {
            SageConstants.Api.WareHouseCodes.Eu => (WareHouseConstants.WareHouseCodes.Eu,
                WareHouseConstants.Locations.EuropeStandard),
            SageConstants.Api.WareHouseCodes.EuBStock => (WareHouseConstants.WareHouseCodes.EuBStock,
                WareHouseConstants.Locations.EuropeStandardB),
            SageConstants.Api.WareHouseCodes.Us => (WareHouseConstants.WareHouseCodes.Us,
                WareHouseConstants.Locations.NorthAmericaStandard),
            _ => (string.Empty, string.Empty)
        };
    }

    public static string SageCurrencyCodeToNorcePriceListCode(string currencyCode)
    {
        return currencyCode switch
        {
            SageConstants.Fields.Currency.EUR => PriceListsConstants.PriceListCodes.EuropeStandard,
            SageConstants.Fields.Currency.USD => PriceListsConstants.PriceListCodes.USStandard,
            SageConstants.Fields.Currency.SEK => PriceListsConstants.PriceListCodes.SwedenStandard,
            SageConstants.Fields.Currency.DKK => PriceListsConstants.PriceListCodes.DenmarkStandard,
            SageConstants.Fields.Currency.NOK => PriceListsConstants.PriceListCodes.NorwayStandard,
            SageConstants.Fields.Currency.GBP => PriceListsConstants.PriceListCodes.GreatBritainStandard,

            _ => string.Empty
        };
    }
}