namespace SharedLib.Constants;
// ReSharper disable InconsistentNaming

public static class SageConstants
{
    public static string SiteUrl = "https://strandbergguitars.com/";
    public static class SageClientConstants
    {
        public const string SoapEnvelope = @"<soapenv:Envelope xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xmlns:xsd=""http://www.w3.org/2001/XMLSchema"" xmlns:soapenv=""http://schemas.xmlsoap.org/soap/envelope/"" xmlns:wss=""http://www.adonix.com/WSS"" xmlns:soapenc=""http://schemas.xmlsoap.org/soap/encoding/"">
                                <soapenv:Header/>
                                  <soapenv:Body>
                                    {0}
                                  </soapenv:Body>
                                </soapenv:Envelope>";
    }

    public static class Api
    {
        public const string ItemEndpoint = "XITM";
        public const string StockEndpoint = "XSTOCKAPI";
        public const string PriceEndpoint = "XPL";

        public static class Languages
        {
            public const string En = "ENG";
        }

        public static class WareHouseCodes
        {
            public const string Eu = "101";
            public const string EuBStock = "101-B";
            public const string Us = "211";
        }
    }

    public static class Fields
    {
        public const string Page = "PAGE";
        public const string TotalPages = "PAGES";
        public static class ProductField
        {
            public const string ManufacturerCode = "BPSNUM";
            public const string ManufacturerName = "XBPSDESCR";
            public const string ManufacturerPartNo = "ITMREFBPS";
            public const string DescriptionEn = "XDESCRENG";
            public const string PartNo = "ITMREF";
            public const string Status = "ITMSTA";
            public const string StatusCodes = "XSTATCODES";
            public const string EanCode = "";
            public const string CSVdata = "XDATA";
            public const string SupplierName = "XBPSDESCR";
            public const string SupplierPartNumber = "ITMREFBPS";

            public static class StatusCode
            {
                public const string Bass = "B";
                public const string SignatureBass = "BS";
                public const string Guitar = "G";
                public const string SignatureGuitar = "GS";
                public const string Misc = "M";
                public const string Services = "X";

                public const string CustomShop = "CS";
                public const string MadeToMeasure = "MM";
                public const string ProductionModel = "PM";
                public const string Special = "SP";
                public const string UsaSelect = "US";

                public const string FourStringBass = "B4";
                public const string FiveStringBass = "B5";
                public const string SixStringBass = "B6";
                public const string SevenStringBass = "B7";
                public const string EightStringBass = "B8";
                public const string NineStringBass = "B9";
                public const string SixStringGuitar = "G6";
                public const string SevenStringGuitar = "G7";
                public const string EightStringGuitar = "G8";
                public const string NineStringGuitar = "G9";

                public const string Fixed = "F";
                public const string Tremolo = "T";

                public const string Boden = "B"; //boden
                public const string Sälen = "S"; //salen
                public const string Varberg = "V"; //varberg
                public const string Meloria = "meloria";
            }
        }

        public static class PriceField
        {
            public const string Site = "SALFCY";
            public const string PartNo = "ITMREF";
            public const string CustomerId = "BPCORD";
            public const string CurrencyCode = "CUR";
            public const string Price = "GROPRI";
            public const string UnitOfMeasurement = "UOM";
            public const string DiscountPercentage = "XDISCOUNT";
        }

        public static class Currency
        {
            public const string EUR = "EUR";
            public const string USD = "USD";
            public const string SEK = "SEK";
            public const string DKK = "DKK";
            public const string NOK = "NOK";
            public const string GBP = "GBP";
            public const string JPY = "JPY";
        }

        public static class StockField
        {
            public const string PartNo = "ITEM_NO";
            public const string Quantity = "QUANTITY";
            public const string Unit = "UNIT";
        }
        
        
    }

}