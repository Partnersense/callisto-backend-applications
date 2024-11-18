using SharedLib.Constants;
using SharedLib.Constants.NorceConstants;

namespace SharedLib.Util;
public static class MarketUtil
{
    public static string GetWarehouseCodeByMarket(string market, bool bStock)
    {
        return (market, bStock) switch
        {
            (LanguageConstants.Market.Europe, true) => WareHouseConstants.WareHouseCodes.EuBStock,
            (LanguageConstants.Market.USA, _) => WareHouseConstants.WareHouseCodes.Us,
            _ => WareHouseConstants.WareHouseCodes.Eu
        };
    }
}
