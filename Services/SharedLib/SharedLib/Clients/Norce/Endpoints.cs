namespace SharedLib.Clients.Norce;

public static class Endpoints
{
    /// <summary>
    /// Concatenates the service base url with the service endpoint
    /// </summary>
    /// <param name="baseUrl"></param>
    /// <param name="serviceEndpoint"></param>
    /// <returns></returns>
    public static string GetEndpoint(string baseUrl, string serviceEndpoint)
    {
        if (serviceEndpoint.Contains(baseUrl))
            return serviceEndpoint;

        var combinedUrl = $"{baseUrl.TrimEnd('/')}/{serviceEndpoint.TrimStart('/')}";
        return combinedUrl;
    }

    public const string Auth = "/identity/1.0/connect/token";

    public static class Api
    {
        public static class Metadata
        {
            public const string Base = "commerce/metadata/1.1/";
            public static string GetApplication(string cultureCode) => $"{Base}GetApplication?format=json&cultureCode={cultureCode}";
        }
        public static class Product
        {
            public const string Base = "commerce/product/1.1/";

            public static string ListStatuses(string cultureCode) =>
                $"{Base}ListStatuses?format=json&cultureCode={cultureCode}";

            public static string GetProductByPartNo(string partNo) =>
                $"{Base}GetProductByPartNo?format=json&partNo={partNo}";

            public static string ListFlags(string? cultureCode = "") =>
                $"{Base}ListFlags?format=json&cultureCode={cultureCode}";

            public static string ListProducts2(List<int> statusSeed, List<int> pricelistSeed, int SalesAreaId, int pageNumber, int pageSize) =>
                $"{Base}ListProducts2?format=json&statusSeed={string.Join(",", statusSeed)}&pricelistSeed={string.Join(",", pricelistSeed)}&salesAreaId={SalesAreaId}&pageNo={pageNumber}&pageSize={pageSize}";
        }
        public static class Shopping
        {
            public const string Base = "commerce/shopping/1.1/";
            public static string GetBasket(string id, string? pricelistId, string? currencyId, string? cultureCode) =>
                $"{Base}GetBasket?format=json&id={id}&pricelistSeed={pricelistId}&currencyId={currencyId}&cultureCode={cultureCode}";
        }
    }

    public static class Connect
    {
        public const string Base = "commerce/connect/4.0/";

        public static class Product
        {
            public const string ImportProducts = $"{Base}Product/ImportProducts";
            public const string ImportOnhands = $"{Base}Product/ImportOnhands";
            public const string ImportPriceLists = $"{Base}Product/ImportPriceLists";
            public const string ImportSkuPriceLists = $"{Base}Product/ImportSkuPriceLists";
        }

        public static class Job
        {
            public static string GetJob(string jobId) => $"{Base}api/Job/Get?id={jobId}";
            public const string Ping = $"{Base}Job/Ping";
        }

        public static class Supplier
        {
            public const string ImportProducts = $"{Base}Supplier/ImportProducts";
        }

        public static class Order
        {
            public const string CancelOrder = $"{Base}Order/CancelOrder";
            public const string CreateDeliveryNote = $"{Base}Order/CreateDeliveryNote";
            public const string CreateInvoice = $"{Base}Order/CreateInvoice";
            public const string CreditPayment = $"{Base}Order/CreditPayment";
            public const string SendOrderStatus = $"{Base}Order/SendOrderStatus";
            public const string SendPosShipment = $"{Base}Order/SendPosShipment";
        }

        public static class Customer
        {
            public const string ImportCompanies = $"{Base}Customer/ImportCompanies";
            public const string ImportCustomers = $"{Base}Customer/ImportCustomers";
        }
    }

    public static class Query
    {
        public const string Base = "commerce/query/2.0/";
        public static class Application
        {
            public const string Parametrics = $"{Base}Application/Parametrics";
            public const string ParametricsWithCultures = $"{Base}Application/Parametrics?$expand=Cultures($select=*)";
            public const string Cultures = $"{Base}Application/ApplicationCultures";
            public const string ClientCurrencies = $"{Base}Application/ClientCurrencies";
            public const string Countries = $"{Base}Application/ApplicationCountries";
            public const string Categories = $"{Base}Application/Categories";
            public const string CategoryHierarchy = $"{Base}Application/CategoryHierarchy";
            public const string CategoryStructureItems = $"{Base}Application/CategoryStructureItems";
            public const string CategoryStructures = $"{Base}Application/CategoryStructures";
            public const string ClientCultures = $"{Base}Application/ClientCultures";
            public const string ClientManufacturers = $"{Base}Application/ClientManufacturers";
            public const string ClientPriceLists = $"{Base}Application/ClientPriceLists";
            public const string ClientPriceListsIncPriceList = $"{Base}Application/ClientPriceLists?$expand=PriceList";
            public const string ClientSuppliers = $"{Base}Application/ClientSuppliers";
            public const string ClientSalesAreas = $"{Base}Application/SalesAreas";
            public const string ClientWarehouses = $"{Base}Application/ClientWarehouses";
            public const string CustomerInfoTypes = $"{Base}Application/CustomerInfoTypes";
            public const string FileInfos = $"{Base}Application/FileInfos";
            public const string OrderInfoTypes = $"{Base}Application/OrderInfoTypes";
            public const string ParametricListValues = $"{Base}Application/ParametricListValues";
            public const string ParametricMultipleValues = $"{Base}Application/ParametricMultipleValues";
            public const string ProductFlags = $"{Base}Application/ProductFlags";
            public const string ProductInfoTypes = $"{Base}Application/ProductInfoTypes";
            public const string ProductRelationTypes = $"{Base}Application/ProductRelationTypes";
            public const string VariantGroups = $"{Base}Application/VariantGroups";
            public const string MetaData = $"{Base}Application/$metadata";
            public static string ApplicationPriceLists(int applicationId) => $"{Base}Application/ApplicationPriceLists?$filter=ApplicationId eq {applicationId}";
        }

        public static class Core
        {
            public const string Currencies = $"{Base}Core/Currencies";
            public const string Applications = $"{Base}Core/Applications";
            public const string Countries = $"{Base}Core/Countries";
            public const string DeliveryMethods = $"{Base}Core/DeliveryMethods";
            public const string FileTypes = $"{Base}Core/FileTypes";
            public const string Files = $"{Base}Core/Files";
            public const string VatCodes = $"{Base}Core/VatCodes";
            public const string SkuStatuses = $"{Base}Core/SkuStatuses";
            public const string ProductInfoTypeGroups = $"{Base}Core/ProductInfoTypeGroups";
            public const string PaymentMethods = $"{Base}Core/PaymentMethods";
        }

        public static class Order
        {
            public const string FullOrder = $"{Base}Orders/Orders?$expand=Items,Payer,Buyer,ShipTo,Infos";
        }

        public static class Invoice
        {
            public const string FullInvoice = $"{Base}Orders/Invoices?$expand=*&select=*";
        }

        public static class Sku
        {
            public const string SkuByPartno = $"{Base}Products/ProductSkus";

            public const string SkuPriceEnrichment =
                $"{Base}Products/ProductSkuPriceLists?$filter=IsActive eq true and QtyBreak eq 1&$select=PartNo,PriceListId,CurrencyId,PriceSale,PriceCatalog,IsPriceDiscountable";

            public static string GetSkuPrimaryPrice(string partNo, int priceListId) =>
                $"{Base}Products/ProductSkuPriceLists?$filter=PartNo eq '{partNo}' and PriceListId eq {priceListId} and IsActive eq true and QtyBreak eq 1&$select=PartNo,PriceListId,PriceSale,PriceStandard";

            public static string GetAllSkuPrices(int priceListId) =>
                $"{Base}Products/ProductSkuPriceLists?$filter=PriceListId eq {priceListId} and IsActive eq true and QtyBreak eq 1&$select=PartNo,PriceListId,PriceSale,PriceStandard";
        }
    }
}
