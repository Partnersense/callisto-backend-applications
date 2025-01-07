using System.ComponentModel.DataAnnotations;

namespace FeedService.Domain.Models
{
    public class PriceProduct
    {
        /// <summary>
        /// The unique identifier for the product variant in the sales area.
        /// Maps to the 'Id' field in the source data.
        /// </summary>
        public required int Id { get; init; }

        /// <summary>
        /// The product's unique part number in the sales area.
        /// Maps to the 'PartNo' field in the source data.
        /// </summary>
        public required string PartNo { get; init; }

        /// <summary>
        /// The name of the product specific to this sales area.
        /// Maps to the 'Name' field in the source data.
        /// </summary>
        public required string Name { get; init; }

        /// <summary>
        /// The base price excluding VAT in the sales area's currency.
        /// Maps to the 'Price' field in the source data.
        /// </summary>
        public required decimal Price { get; init; }

        /// <summary>
        /// The price including VAT in the sales area's currency.
        /// Maps to the 'PriceIncVat' field in the source data.
        /// </summary>
        public required decimal PriceIncVat { get; init; }

        /// <summary>
        /// The recommended retail price (RRP) if available.
        /// Maps to the 'PriceRecommended' field in the source data.
        /// Can be null if no recommended price exists.
        /// </summary>
        public decimal? PriceRecommended { get; init; }

        /// <summary>
        /// The catalog list price if available.
        /// Maps to the 'PriceCatalog' field in the source data.
        /// Can be null if no catalog price exists.
        /// </summary>
        public decimal? PriceCatalog { get; init; }

        /// <summary>
        /// The VAT rate as a multiplier (e.g., 1.25 for 25% VAT).
        /// Maps to the 'VatRate' field in the source data.
        /// </summary>
        public decimal VatRate { get; init; }

        /// <summary>
        /// Current stock quantity available in the sales area.
        /// Maps to the 'OnHand.Value' field in the source data.
        /// </summary>
        public required decimal Stock { get; init; }

        /// <summary>
        /// Expected incoming stock quantity.
        /// Maps to the 'OnHand.IncomingValue' field in the source data.
        /// </summary>
        public decimal IncomingStock { get; init; }

        /// <summary>
        /// The date when the next stock delivery is expected.
        /// Maps to the 'OnHand.NextDeliveryDate' field in the source data.
        /// Can be null if no delivery is scheduled.
        /// </summary>
        public DateTime? NextDeliveryDate { get; init; }

        /// <summary>
        /// Number of days required for delivery.
        /// Maps to the 'OnHand.LeadTimeDayCount' field in the source data.
        /// Can be null if lead time is not specified.
        /// </summary>
        public int? LeadTimeDayCount { get; init; }

        /// <summary>
        /// Whether the stock information is currently active and valid.
        /// Maps to the 'OnHand.IsActive' field in the source data.
        /// </summary>
        public required bool IsStockActive { get; init; }

        /// <summary>
        /// When the stock information was last checked.
        /// Maps to the 'OnHand.LastChecked' field in the source data.
        /// Can be null if stock has never been checked.
        /// </summary>
        public DateTime? LastChecked { get; init; }

        /// <summary>
        /// Whether the product can be returned in this sales area.
        /// Maps to the 'OnHand.IsReturnable' field in the source data.
        /// </summary>
        public bool IsReturnable { get; init; }

        /// <summary>
        /// The recommended purchase quantity for this product.
        /// Maps to the 'RecommendedQuantity' field in the source data.
        /// </summary>
        public decimal RecommendedQuantity { get; init; }

        /// <summary>
        /// Whether the recommended quantity is fixed or can be modified.
        /// Maps to the 'IsRecommendedQuantityFixed' field in the source data.
        /// </summary>
        public bool IsRecommendedQuantityFixed { get; init; }

        /// <summary>
        /// Whether the product can be purchased in this sales area.
        /// Maps to the 'IsBuyable' field in the source data.
        /// </summary>
        public required bool IsBuyable { get; init; }

        /// <summary>
        /// When the price and stock information was last updated.
        /// Maps to the 'Updated' field in the source data.
        /// Can be null if never updated.
        /// </summary>
        public DateTime? Updated { get; init; }
    }
}
