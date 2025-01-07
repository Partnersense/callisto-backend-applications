namespace FeedService.Domain.DTOs.External.DataFeedWatch;

public class DataFeedWatchDto
{
    // Core product properties
    public required string Id { get; set; }
    public required string Title { get; set; }
    public string? Description { get; set; }
    public string? ProductLink { get; set; }
    public string? ImageLink { get; set; }
    public List<string> AdditionalImages { get; set; } = [];
    public string? Price { get; set; }
    public string Currency { get; set; } = string.Empty;
    public string? Availability { get; set; }
    public string? Category { get; set; }
    public string? EanCode { get; set; }

    // Manufacturer details
    public string? ManufacturerName { get; set; }
    public string? ManufacturerCode { get; set; }
    public string? ManufacturerPartNo { get; set; }

    // Additional variant properties
    public string? Status { get; set; }
    public string? Size { get; set; }
    public string? Color { get; set; }

    // Logistics information
    public double? Width { get; set; }
    public double? Height { get; set; }
    public double? Depth { get; set; }
    public double? Weight { get; set; }
    public string? CommodityCode { get; set; }
    public int? RecommendedQuantity { get; set; }
    public bool IsRecommendedQuantityFixed { get; set; }

    // Price related fields
    public bool IsDiscountable { get; set; }
    public double? OriginalPrice { get; set; }
    public double? VatRate { get; set; }
    public double? PurchaseCost { get; set; }
    public double? UnitCost { get; set; }
    public double? RecommendedPrice { get; set; }
    public double? StandardPrice { get; set; }
    public double? CatalogPrice { get; set; }

    // Other product details
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public string? Type { get; set; }

    // Flags and parametrics
    public Dictionary<string, string>? ProductFlags { get; set; }
    public Dictionary<string, string>? VariantFlags { get; set; }
    public Dictionary<string, string>? Parametrics { get; set; }

    // SEO and content
    public string? SubHeader { get; set; }
    public string? SubDescription { get; set; }
    public string? UniqueUrlName { get; set; }
}