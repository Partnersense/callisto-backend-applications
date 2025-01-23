using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedLib.Models.Norce.Api
{
    /// <summary>
    /// Represents a ListProducts2 Product Response
    /// </summary>
    public class ListProducts2Response
    {
        /// <summary>
        /// The unique identifier of the product
        /// <br/><br/>
        /// Example Values: 238594
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// The name of the product
        /// <br/><br/>
        /// Example Values: "3D Floral Sleeve Short Dress Svart"
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The sub-header of the product
        /// <br/><br/>
        /// Example Values: ""
        /// </summary>
        public string SubHeader { get; set; }

        /// <summary>
        /// The manufacturer information of the product
        /// <br/><br/>
        /// Example Values: { Id: 292, Name: "Happy Holly" }
        /// </summary>
        public ManufacturerInfo Manufacturer { get; set; }

        /// <summary>
        /// The image identifier of the product
        /// <br/><br/>
        /// Example Values: "0ff9e168-26c3-44cd-8b17-a72b2df9242f"
        /// </summary>
        public string Image { get; set; }

        /// <summary>
        /// The campaign image identifier of the product
        /// <br/><br/>
        /// Example Values: null
        /// </summary>
        public string? CampaignImage { get; set; }

        /// <summary>
        /// The large image identifier of the product
        /// <br/><br/>
        /// Example Values: null
        /// </summary>
        public string? LargeImage { get; set; }

        /// <summary>
        /// The thumbnail image identifier of the product
        /// <br/><br/>
        /// Example Values: null
        /// </summary>
        public string? ThumbnailImage { get; set; }

        /// <summary>
        /// Comma-separated list of flag IDs
        /// <br/><br/>
        /// Example Values: "4,5,6"
        /// </summary>
        public string FlagIdSeed { get; set; }

        /// <summary>
        /// The price of the product
        /// <br/><br/>
        /// Example Values: 299.00
        /// </summary>
        public decimal Price { get; set; }

        /// <summary>
        /// The recommended price of the product
        /// <br/><br/>
        /// Example Values: null
        /// </summary>
        public decimal? PriceRecommended { get; set; }

        /// <summary>
        /// The catalog price of the product
        /// <br/><br/>
        /// Example Values: null
        /// </summary>
        public decimal? PriceCatalog { get; set; }

        /// <summary>
        /// The VAT rate for the product
        /// <br/><br/>
        /// Example Values: 1.25
        /// </summary>
        public decimal VatRate { get; set; }

        /// <summary>
        /// The recommended quantity for the product
        /// <br/><br/>
        /// Example Values: 1.000
        /// </summary>
        public decimal RecommendedQuantity { get; set; }

        /// <summary>
        /// Stock information for the main warehouse
        /// </summary>
        public StockInfo OnHand { get; set; }

        /// <summary>
        /// Stock information for the store
        /// </summary>
        public StockInfo OnHandStore { get; set; }

        /// <summary>
        /// Stock information for the supplier
        /// </summary>
        public StockInfo OnHandSupplier { get; set; }

        /// <summary>
        /// The unique key of the product
        /// <br/><br/>
        /// Example Values: "15931eb0-79cc-4a5a-8c89-25dd7d92c09d"
        /// </summary>
        public Guid Key { get; set; }

        /// <summary>
        /// The last update timestamp
        /// <br/><br/>
        /// Example Values: null
        /// </summary>
        public DateTime? Updated { get; set; }

        /// <summary>
        /// The image key of the product
        /// <br/><br/>
        /// Example Values: "e6ad86de-6787-40ce-b351-d62e004d9c48"
        /// </summary>
        public string ImageKey { get; set; }

        /// <summary>
        /// The popularity rank of the product
        /// <br/><br/>
        /// Example Values: 2147483647
        /// </summary>
        public int PopularityRank { get; set; }

        /// <summary>
        /// The status identifier of the product
        /// <br/><br/>
        /// Example Values: 1
        /// </summary>
        public int StatusId { get; set; }

        /// <summary>
        /// The name of the variant
        /// <br/><br/>
        /// Example Values: null
        /// </summary>
        public string? VariantName { get; set; }

        /// <summary>
        /// The image key of the variant
        /// <br/><br/>
        /// Example Values: null
        /// </summary>
        public string? VariantImageKey { get; set; }

        /// <summary>
        /// Comma-separated list of additional image keys
        /// <br/><br/>
        /// Example Values: "98:ad46cf18-c1ac-4ec2-95cb-620a289e62b2,98:b8f15bb9-f314-4853-9467-34d912104a85,98:a6549da3-3a29-4713-8bc8-9af5ad9ec5e5"
        /// </summary>
        public string AdditionalImageKeySeed { get; set; }

        /// <summary>
        /// The key used for grouping variants
        /// <br/><br/>
        /// Example Values: "v40548"
        /// </summary>
        public string GroupByKey { get; set; }

        /// <summary>
        /// Comma-separated list of variant flag IDs
        /// <br/><br/>
        /// Example Values: null
        /// </summary>
        public string? VariantFlagIdSeed { get; set; }

        /// <summary>
        /// The part number
        /// <br/><br/>
        /// Example Values: null
        /// </summary>
        public string? PartNo { get; set; }

        /// <summary>
        /// The price list identifier
        /// <br/><br/>
        /// Example Values: null
        /// </summary>
        public int? PriceListId { get; set; }

        /// <summary>
        /// The sort order of the product
        /// <br/><br/>
        /// Example Values: 2147483647
        /// </summary>
        public int SortOrder { get; set; }

        /// <summary>
        /// The category identifier
        /// <br/><br/>
        /// Example Values: 2292
        /// </summary>
        public int CategoryId { get; set; }

        /// <summary>
        /// Comma-separated list of parametric values
        /// <br/><br/>
        /// Example Values: "40:543,41:570"
        /// </summary>
        public string ParametricListSeed { get; set; }

        /// <summary>
        /// Comma-separated list of parametric multiple values
        /// <br/><br/>
        /// Example Values: ""
        /// </summary>
        public string ParametricMultipleSeed { get; set; }

        /// <summary>
        /// Comma-separated list of parametric values
        /// <br/><br/>
        /// Example Values: ""
        /// </summary>
        public string ParametricValueSeed { get; set; }

        /// <summary>
        /// Array of parametric text fields
        /// <br/><br/>
        /// Example Values: []
        /// </summary>
        public string[] ParametricTextField { get; set; }

        /// <summary>
        /// Comma-separated list of variant parametric values
        /// <br/><br/>
        /// Example Values: "40,41"
        /// </summary>
        public string VariantParametricSeed { get; set; }

        /// <summary>
        /// The unique URL friendly name of the product
        /// <br/><br/>
        /// Example Values: "3d-floral-sleeve-short-dress-svart"
        /// </summary>
        public string UniqueName { get; set; }

        /// <summary>
        /// The stock display break point
        /// <br/><br/>
        /// Example Values: null
        /// </summary>
        public int? StockDisplayBreakPoint { get; set; }

        /// <summary>
        /// Indicates if the product is buyable
        /// <br/><br/>
        /// Example Values: true
        /// </summary>
        public bool IsBuyable { get; set; }

        /// <summary>
        /// The sub description of the product
        /// <br/><br/>
        /// Example Values: ""
        /// </summary>
        public string SubDescription { get; set; }

        /// <summary>
        /// The quantity
        /// <br/><br/>
        /// Example Values: null
        /// </summary>
        public decimal? Quantity { get; set; }

        /// <summary>
        /// The type of the product
        /// <br/><br/>
        /// Example Values: 1
        /// </summary>
        public int Type { get; set; }

        /// <summary>
        /// Comma-separated list of category IDs
        /// <br/><br/>
        /// Example Values: "2292"
        /// </summary>
        public string CategoryIdSeed { get; set; }

        /// <summary>
        /// Indicates if the recommended quantity is fixed
        /// <br/><br/>
        /// Example Values: false
        /// </summary>
        public bool IsRecommendedQuantityFixed { get; set; }

        /// <summary>
        /// The product synonyms
        /// <br/><br/>
        /// Example Values: null
        /// </summary>
        public string? Synonyms { get; set; }

        /// <summary>
        /// The unique URL friendly name of the variant
        /// <br/><br/>
        /// Example Values: "3d-floral-sleeve-short-dress-svart-238594"
        /// </summary>
        public string VariantUniqueName { get; set; }

        /// <summary>
        /// Indicates if the product is subscribable
        /// <br/><br/>
        /// Example Values: false
        /// </summary>
        public bool IsSubscribable { get; set; }

        /// <summary>
        /// The unit of measurement
        /// <br/><br/>
        /// Example Values: "st"
        /// </summary>
        public string UnitOfMeasurement { get; set; }

        /// <summary>
        /// The unit of measurement count
        /// <br/><br/>
        /// Example Values: 1.000
        /// </summary>
        public decimal UnitOfMeasurementCount { get; set; }

        /// <summary>
        /// The EAN code
        /// <br/><br/>
        /// Example Values: null
        /// </summary>
        public string? EanCode { get; set; }

        /// <summary>
        /// The standard price
        /// <br/><br/>
        /// Example Values: null
        /// </summary>
        public decimal? PriceStandard { get; set; }

        /// <summary>
        /// Indicates if the product contains dangerous goods
        /// <br/><br/>
        /// Example Values: false
        /// </summary>
        public bool IsDangerousGoods { get; set; }

        /// <summary>
        /// The price including VAT
        /// <br/><br/>
        /// Example Values: 373.75
        /// </summary>
        public decimal PriceIncVat { get; set; }
    }

    /// <summary>
    /// Represents manufacturer information for a product
    /// </summary>
    public class ManufacturerInfo
    {
        /// <summary>
        /// The unique identifier of the manufacturer
        /// <br/><br/>
        /// Example Values: 292
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// The name of the manufacturer
        /// <br/><br/>
        /// Example Values: "Happy Holly"
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The part number from the manufacturer
        /// <br/><br/>
        /// Example Values: null
        /// </summary>
        public string? PartNo { get; set; }

        /// <summary>
        /// The logo path of the manufacturer
        /// <br/><br/>
        /// Example Values: ""
        /// </summary>
        public string LogoPath { get; set; }

        /// <summary>
        /// The logo key of the manufacturer
        /// <br/><br/>
        /// Example Values: null
        /// </summary>
        public string? LogoKey { get; set; }

        /// <summary>
        /// The unique name of the manufacturer
        /// <br/><br/>
        /// Example Values: "happy-holly"
        /// </summary>
        public string UniqueName { get; set; }
    }

    /// <summary>
    /// Represents stock information for a product
    /// </summary>
    public class StockInfo
    {
        /// <summary>
        /// The current stock value
        /// <br/><br/>
        /// Example Values: 501.000
        /// </summary>
        public decimal Value { get; set; }

        /// <summary>
        /// The incoming stock value
        /// <br/><br/>
        /// Example Values: 0.000
        /// </summary>
        public decimal IncomingValue { get; set; }

        /// <summary>
        /// The next delivery date
        /// <br/><br/>
        /// Example Values: null
        /// </summary>
        public DateTime? NextDeliveryDate { get; set; }

        /// <summary>
        /// The lead time in days
        /// <br/><br/>
        /// Example Values: 2
        /// </summary>
        public int? LeadtimeDayCount { get; set; }

        /// <summary>
        /// The last check timestamp
        /// <br/><br/>
        /// Example Values: "/Date(1731440148857+0000)/"
        /// </summary>
        public DateTime? LastChecked { get; set; }

        /// <summary>
        /// Indicates if the stock location is active
        /// <br/><br/>
        /// Example Values: true
        /// </summary>
        public bool IsActive { get; set; }

        /// <summary>
        /// Indicates if returns are allowed
        /// <br/><br/>
        /// Example Values: false
        /// </summary>
        public bool IsReturnable { get; set; }

        /// <summary>
        /// Additional stock information
        /// <br/><br/>
        /// Example Values: null
        /// </summary>
        public object? Info { get; set; }
    }
}
