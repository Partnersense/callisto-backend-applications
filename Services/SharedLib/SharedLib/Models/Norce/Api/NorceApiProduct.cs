namespace SharedLib.Models.Norce.Api;

// Uncommented field are left out to make it easier to include for future operations
// Fields that are added should have their data types verified

public class NorceApiProduct
{
    public int Id { get; set; }
    //public string Name { get; set; }
    //public string Description { get; set; }
    public string PartNo { get; set; } = string.Empty;
    //public string SubHeader { get; set; }
    //public Manufacturer Manufacturer { get; set; }
    //public object Image { get; set; }
    //public object CampaignImage { get; set; }
    //public object LargeImage { get; set; }
    //public object ThumbnailImage { get; set; }
    //public object[] Files { get; set; }
    public string FlagIdSeed { get; set; }= string.Empty;
    //public float Price { get; set; }
    //public object PriceCatalog { get; set; }
    //public object PriceRecommended { get; set; }
    //public object PriceFreight { get; set; }
    //public object PriceFreightVatRate { get; set; }
    //public int VatRate { get; set; }
    //public float RecommendedQuantity { get; set; }
    //public Onhand OnHand { get; set; }
    //public Onhandstore OnHandStore { get; set; }
    //public Onhandsupplier OnHandSupplier { get; set; }
    public List<NorceApiVariant> Variants { get; set; } = [];
    //public int PriceListId { get; set; }
    //public string Key { get; set; }
    //public DateTime Updated { get; set; }
    //public object NavigationNodeKey { get; set; }
    //public int CategoryId { get; set; }
    //public string CategoryName { get; set; }
    //public object ImageKey { get; set; }
    //public object[] VariantParametrics { get; set; }
    //public int StatusId { get; set; }
    //public string MetaTags { get; set; }
    //public string MetaDescription { get; set; }
    //public object VariantName { get; set; }
    //public string DescriptionHeader { get; set; }
    //public string UniqueName { get; set; }
    //public object StockDisplayBreakPoint { get; set; }
    //public object[] Parametrics { get; set; }
    //public object[] Families { get; set; }
    //public bool IsBuyable { get; set; }
    //public string SubDescription { get; set; }
    //public string Uom { get; set; }
    //public float UomCount { get; set; }
    //public object EanCode { get; set; }
    //public int Type { get; set; }
    //public Category[] Categories { get; set; }
    //public bool IsRecommendedQuantityFixed { get; set; }
    //public object PopularityRank { get; set; }
    //public object CostPurchase { get; set; }
    //public object CostUnit { get; set; }
    //public object Title { get; set; }
    //public float ActualWeight { get; set; }
    //public object CommodityCode { get; set; }
    //public bool IsDropShipOnly { get; set; }
    //public object Synonyms { get; set; }
    //public bool IsSubscribable { get; set; }
    //public object UnspscCode { get; set; }
    //public object PriceStandard { get; set; }
    //public object Width { get; set; }
    //public object Height { get; set; }
    //public object Depth { get; set; }
    //public bool IsDangerousGoods { get; set; }
    //public bool HasQuantityBreaks { get; set; }
    //public string GroupByKey { get; set; }
    //public float PriceIncVat { get; set; }
}

//public class Manufacturer
//{
//    public int Id { get; set; }
//    public object Name { get; set; }
//    public string PartNo { get; set; }
//    public object LogoPath { get; set; }
//    public object LogoKey { get; set; }
//    public object UniqueName { get; set; }
//}

//public class Onhand
//{
    //public decimal Value { get; set; }
    //public decimal IncomingValue { get; set; }
    //public object NextDeliveryDate { get; set; }
    //public object LeadtimeDayCount { get; set; }
    //public object LastChecked { get; set; }
    //public bool IsActive { get; set; }
    //public bool IsReturnable { get; set; }
    //public object Info { get; set; }
//}

//public class Onhandstore
//{
//    public float Value { get; set; }
//    public float IncomingValue { get; set; }
//    public object NextDeliveryDate { get; set; }
//    public object LeadtimeDayCount { get; set; }
//    public object LastChecked { get; set; }
//    public bool IsActive { get; set; }
//    public bool IsReturnable { get; set; }
//    public object Info { get; set; }
//}

//public class Onhandsupplier
//{
//    public float Value { get; set; }
//    public float IncomingValue { get; set; }
//    public object NextDeliveryDate { get; set; }
//    public object LeadtimeDayCount { get; set; }
//    public object LastChecked { get; set; }
//    public bool IsActive { get; set; }
//    public bool IsReturnable { get; set; }
//    public object Info { get; set; }
//}

public class NorceApiVariant
{
    //    public int Id { get; set; }
    //    public string Name { get; set; }
    //    public string Description { get; set; }
    public string PartNo { get; set; } = string.Empty;
    //    public string SubHeader { get; set; }
    //    public Manufacturer1 Manufacturer { get; set; }
    //    public object Image { get; set; }
    //    public object CampaignImage { get; set; }
    //    public object LargeImage { get; set; }
    //    public object ThumbnailImage { get; set; }
    //    public object[] Files { get; set; }
    public string FlagIdSeed { get; set; } = string.Empty;
    //    public float Price { get; set; }
    //    public object PriceCatalog { get; set; }
    //    public object PriceRecommended { get; set; }
    //    public object PriceFreight { get; set; }
    //    public object PriceFreightVatRate { get; set; }
    //    public int VatRate { get; set; }
    //    public float RecommendedQuantity { get; set; }
    //    public Onhand OnHand { get; set; } = new();
    //    public Onhandstore1 OnHandStore { get; set; }
    //    public Onhandsupplier1 OnHandSupplier { get; set; }
    //    public object Variants { get; set; }
    //    public int PriceListId { get; set; }
    //    public string Key { get; set; }
    //    public DateTime Updated { get; set; }
    //    public object NavigationNodeKey { get; set; }
    //    public object CategoryId { get; set; }
    //    public object CategoryName { get; set; }
    //    public object ImageKey { get; set; }
    //    public object[] VariantParametrics { get; set; }
    //    public int StatusId { get; set; }
    //    public string MetaTags { get; set; }
    //    public string MetaDescription { get; set; }
    //    public string VariantName { get; set; }
    //    public string DescriptionHeader { get; set; }
    //    public string UniqueName { get; set; }
    //    public object StockDisplayBreakPoint { get; set; }
    //    public object[] Parametrics { get; set; }
    //    public object Families { get; set; }
    //    public bool IsBuyable { get; set; }
    //    public string SubDescription { get; set; }
    //    public string Uom { get; set; }
    //    public float UomCount { get; set; }
    //    public object EanCode { get; set; }
    //    public int Type { get; set; }
    //    public object Categories { get; set; }
    //    public bool IsRecommendedQuantityFixed { get; set; }
    //    public int PopularityRank { get; set; }
    //    public object CostPurchase { get; set; }
    //    public object CostUnit { get; set; }
    //    public object Title { get; set; }
    //    public float ActualWeight { get; set; }
    //    public object CommodityCode { get; set; }
    //    public bool IsDropShipOnly { get; set; }
    //    public object Synonyms { get; set; }
    //    public bool IsSubscribable { get; set; }
    //    public object UnspscCode { get; set; }
    //    public object PriceStandard { get; set; }
    //    public object Width { get; set; }
    //    public object Height { get; set; }
    //    public object Depth { get; set; }
    //    public bool IsDangerousGoods { get; set; }
    //    public bool HasQuantityBreaks { get; set; }
    //    public string GroupByKey { get; set; }
    //    public float PriceIncVat { get; set; }
}

//public class Manufacturer1
//{
//    public int Id { get; set; }
//    public object Name { get; set; }
//    public string PartNo { get; set; }
//    public object LogoPath { get; set; }
//    public object LogoKey { get; set; }
//    public object UniqueName { get; set; }
//}

//public class Onhandstore1
//{
//    public float Value { get; set; }
//    public float IncomingValue { get; set; }
//    public object NextDeliveryDate { get; set; }
//    public object LeadtimeDayCount { get; set; }
//    public object LastChecked { get; set; }
//    public bool IsActive { get; set; }
//    public bool IsReturnable { get; set; }
//    public object Info { get; set; }
//}

//public class Onhandsupplier1
//{
//    public float Value { get; set; }
//    public float IncomingValue { get; set; }
//    public object NextDeliveryDate { get; set; }
//    public object LeadtimeDayCount { get; set; }
//    public object LastChecked { get; set; }
//    public bool IsActive { get; set; }
//    public bool IsReturnable { get; set; }
//    public object Info { get; set; }
//}

//public class Category
//{
//    public int Id { get; set; }
//    public string Value { get; set; }
//    public string Code { get; set; }
//}
