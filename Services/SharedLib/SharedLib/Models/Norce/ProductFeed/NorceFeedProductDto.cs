using System.Text.Json.Serialization;

namespace FeedService.Domain.Norce;

public class AvailableOnWarehouse
{
    [JsonPropertyName("code")] 
    public string Code { get; set; } = string.Empty;

    [JsonPropertyName("locationCode")] 
    public string LocationCode { get; set; } = string.Empty;
}

public class Culture
{
    [JsonPropertyName("cultureCode")] 
    public string CultureCode { get; set; } = string.Empty;

    [JsonPropertyName("name")] 
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("fullName")] 
    public string FullName { get; set; } = string.Empty;

    [JsonPropertyName("synonyms")] 
    public string? Synonyms { get; set; }

    [JsonPropertyName("groupName")] 
    public string? GroupName { get; set; }

    [JsonPropertyName("unitOfMeasurement")] 
    public string? UnitOfMeasurement { get; set; }

    [JsonPropertyName("value")] 
    public object? Value { get; set; }

    [JsonPropertyName("listValue")] 
    public ListValue? ListValue { get; set; }

    [JsonPropertyName("multipleValues")] 
    public List<ListValue>? MultipleValues { get; set; }
}

public class File
{
    [JsonPropertyName("type")] 
    public string? Type { get; set; }

    [JsonPropertyName("code")] 
    public string? Code { get; set; }

    [JsonPropertyName("fileCode")] 
    public string? FileCode { get; set; }

    [JsonPropertyName("key")] 
    public string? Key { get; set; }

    [JsonPropertyName("mimeType")] 
    public string? MimeType { get; set; }

    [JsonPropertyName("url")] 
    public string? Url { get; set; }
}

public class ListValue
{
    [JsonPropertyName("code")] 
    public string Code { get; set; } = string.Empty;

    [JsonPropertyName("name")] 
    public string Name { get; set; } = string.Empty;
}

public class Logistics
{
    [JsonPropertyName("width")] 
    public double? Width { get; set; }

    [JsonPropertyName("height")] 
    public double? Height { get; set; }

    [JsonPropertyName("depth")] 
    public double? Depth { get; set; }

    [JsonPropertyName("weight")] 
    public double? Weight { get; set; }
}

public class Manufacturer
{
    [JsonPropertyName("code")] 
    public string Code { get; set; } = string.Empty;

    [JsonPropertyName("name")] 
    public string Name { get; set; } = string.Empty;
}

public class MultipleValue
{
    [JsonPropertyName("code")] 
    public string Code { get; set; } = string.Empty;

    [JsonPropertyName("name")] 
    public string Name { get; set; } = string.Empty;
}

public class Names
{
    [JsonPropertyName("cultureCode")] 
    public string CultureCode { get; set; } = string.Empty;

    [JsonPropertyName("name")] 
    public string Name { get; set; }

    [JsonPropertyName("uniqueUrlName")] 
    public string? UniqueUrlName { get; set; }
}

public class Parametric
{
    [JsonPropertyName("code")] 
    public string? Code { get; set; }

    [JsonPropertyName("type")] 
    public string? Type { get; set; }

    [JsonPropertyName("sortOrder")] 
    public int? SortOrder { get; set; }

    [JsonPropertyName("cultures")] 
    public List<Culture> Cultures { get; set; } = [];
}

public class Price
{
    [JsonPropertyName("priceCatalog")] 
    public decimal? PriceCatalog;

    [JsonPropertyName("salesArea")] 
    public string? SalesArea { get; set; }

    [JsonPropertyName("priceListCode")] 
    public string PriceListCode { get; set; } = string.Empty;

    [JsonPropertyName("currency")] 
    public string Currency { get; set; } = string.Empty;

    [JsonPropertyName("value")] 
    public double Value { get; set; }

    [JsonPropertyName("isDiscountable")] 
    public bool IsDiscountable { get; set; }

    [JsonPropertyName("original")] 
    public double Original { get; set; }

    [JsonPropertyName("vatRate")] 
    public double VatRate { get; set; }

    [JsonPropertyName("availableOnWarehouses")]
    public List<AvailableOnWarehouse> AvailableOnWarehouses { get; set; } = [];

    [JsonPropertyName("purchaseCost")] 
    public double PurchaseCost { get; set; }

    [JsonPropertyName("unitCost")] 
    public decimal UnitCost { get; set; }

    [JsonPropertyName("isActive")] 
    public bool IsActive { get; set; }

    [JsonPropertyName("valueIncVat")] 
    public decimal ValueIncVat { get; set; }
    
    [JsonPropertyName("recommendedPrice")]  
    public decimal? RecommendedPrice { get; set; }
    
    [JsonPropertyName("recommendedPriceIncVat")]  
    public decimal? RecommendedPriceIncVat { get; set; }
}

public class Category
{
    [JsonPropertyName("code")] 
    public string Code { get; set; } = string.Empty;

    [JsonPropertyName("cultures")] 
    public List<Culture> Cultures { get; set; } = [];
}

public class PrimaryImage
{
    [JsonPropertyName("type")] 
    public string? Type { get; set; }

    [JsonPropertyName("code")] 
    public object? Code { get; set; }

    [JsonPropertyName("fileCode")] 
    public string? FileCode { get; set; }

    [JsonPropertyName("key")] 
    public string? Key { get; set; }

    [JsonPropertyName("mimeType")] 
    public string? MimeType { get; set; }

    [JsonPropertyName("url")] 
    public string? Url { get; set; }
}

public class NorceFeedProductDto
{
    [JsonPropertyName("code")] 
    public string? Code { get; set; }

    [JsonPropertyName("manufacturer")] 
    public Manufacturer? Manufacturer { get; set; }

    [JsonPropertyName("names")]
    public List<Names> Names { get; set; } = [];

    [JsonPropertyName("primaryCategory")] 
    public Category? PrimaryCategory { get; set; }

    [JsonPropertyName("additionalCategories")] 
    public List<Category>? AdditionalCategories { get; set; }

    [JsonPropertyName("variants")] 
    public List<NorceFeedVariant>? Variants { get; set; }

    [JsonPropertyName("families")] 
    public List<object> Families { get; set; } = [];

    [JsonPropertyName("flags")] 
    public List<Flag> Flags { get; set; } = [];

    [JsonPropertyName("parametrics")] 
    public List<Parametric> Parametrics { get; set; } = [];

    [JsonPropertyName("primaryImage")] 
    public PrimaryImage? PrimaryImage { get; set; }

    [JsonPropertyName("files")] 
    public List<File> Files { get; set; } = [];

    [JsonPropertyName("relations")] 
    public List<Relation> Relations { get; set; } = [];

    [JsonPropertyName("texts")] 
    public List<Text> Texts { get; set; } = [];

    [JsonPropertyName("popularities")] 
    public List<object> Popularities { get; set; } = [];
}

public class Relation
{
    [JsonPropertyName("type")] 
    public string? Type { get; set; }

    [JsonPropertyName("partNo")] 
    public string PartNo { get; set; } = string.Empty;

    [JsonPropertyName("code")] 
    public string Code { get; set; } = string.Empty;
}

public class Flag
{
    [JsonPropertyName("code")] 
    public string? Code { get; set; }
    [JsonPropertyName("groupCode")]
    public string? GroupCode { get; set; }
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("cultures")] 
    public List<FlagCulture> Cultures { get; set; } = [];
}

public class FlagCulture
{
    [JsonPropertyName("cultureCode")] 
    public string CultureCode { get; set; } = string.Empty;

    [JsonPropertyName("name")] 
    public string? Name { get; set; }

    [JsonPropertyName("groupName")] 
    public string? GroupName { get; set; }
}

/// <summary>
/// Texts
/// </summary>
public class Text
{
    [JsonPropertyName("cultureCode")] 
    public string CultureCode { get; set; } = string.Empty;

    [JsonPropertyName("descriptionHeader")] 
    public string? DescriptionHeader { get; set; }

    [JsonPropertyName("description")] 
    public string? Description { get; set; }

    [JsonPropertyName("subHeader")] 
    public string? SubHeader { get; set; }

    [JsonPropertyName("subDescription")] 
    public string? SubDescription { get; set; }

    [JsonPropertyName("synonyms")] 
    public object? Synonyms { get; set; }
}

/// <summary>
/// Variant of product
/// </summary>
public class NorceFeedVariant
{
    [JsonPropertyName("partNo")] 
    public string PartNo { get; set; } = string.Empty;

    [JsonPropertyName("id")] 
    public int Id { get; set; }

    [JsonPropertyName("manufacturerPartNo")] 
    public string ManufacturerPartNo { get; set; } = string.Empty;

    [JsonPropertyName("names")] 
    public List<Names> Names { get; set; } = [];

    [JsonPropertyName("eanCode")] 
    public string EanCode { get; set; } = string.Empty;

    [JsonPropertyName("status")] 
    public string Status { get; set; } = string.Empty;

    [JsonPropertyName("prices")] 
    public List<Price> Prices { get; set; } = [];

    [JsonPropertyName("onHands")] 
    public List<OnHandRecord>? OnHands { get; set; }

    [JsonPropertyName("suppliers")] 
    public List<object>? Suppliers { get; set; }

    [JsonPropertyName("flags")] 
    public List<Flag> Flags { get; set; } = [];

    [JsonPropertyName("primaryImage")] 
    public PrimaryImage? PrimaryImage { get; set; }

    [JsonPropertyName("files")] 
    public List<File>? Files { get; set; }

    [JsonPropertyName("variantDefiningParametrics")] 
    public List<Parametric> VariantDefiningParametrics { get; set; } = [];

    [JsonPropertyName("additionalParametrics")] 
    public List<Parametric> AdditionalParametrics { get; set; } = [];

    [JsonPropertyName("relations")] 
    public List<object> Relations { get; set; } = [];

    [JsonPropertyName("texts")] 
    public List<object> Texts { get; set; } = [];

    [JsonPropertyName("logistics")] 
    public Logistics? Logistics { get; set; }

    [JsonPropertyName("commodityCode")] 
    public string? CommodityCode { get; set; }

    [JsonPropertyName("recommendedQty")] 
    public int? RecommendedQty { get; set; }

    [JsonPropertyName("isRecommendedQtyFixed")] 
    public bool IsRecommendedQtyFixed { get; set; }

    [JsonPropertyName("startDate")] 
    public DateTime? StartDate { get; set; }

    [JsonPropertyName("endDate")] 
    public DateTime? EndDate { get; set; }


    public class OnHandRecord
    {
        [JsonPropertyName("warehouse")] 
        public Warehouse? Warehouse { get; set; }

        [JsonPropertyName("warehouseType")] 
        public string WarehouseType { get; set; } = string.Empty;

        [JsonPropertyName("value")] 
        public double? Value { get; set; }

        [JsonPropertyName("leadTimeDayCount")] 
        public int? LeadTimeDayCount { get; set; }

        [JsonPropertyName("availableOnStores")]
        public string[] AvailableOnStores { get; set; } = [];

        [JsonPropertyName("availableOnPricelists")] 
        public string[] AvailableOnPriceLists { get; set; } = [];
    }

    public class Warehouse
    {
        [JsonPropertyName("code")] 
        public string Code { get; set; } = string.Empty;

        [JsonPropertyName("locationCode")] 
        public string LocationCode { get; set; } = string.Empty;
    }
}