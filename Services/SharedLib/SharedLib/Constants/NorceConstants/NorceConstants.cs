namespace SharedLib.Constants.NorceConstants;

public static class NorceConstants
{
    public static class Manufacturer
    {
        public static class Strandberg
        {
            public const string Name = "Strandberg Guitars";
            public const string Code = "strandbergGuitars";
        }
    }
  
    public static class Parametrics
    {
        public const string ArtistSignatureProductCode = "artistSignatureProduct";
        public const string LimitedEditionCode = "limitedEdition";
        public const string InstrumentYearModel = "instrumentYearModel";
        public const string Supplier = "supplier";
        public const string SupplierPartNumber = "supplierPartNumber";
        public const string InstrumentProductionSku = "instrumentProductionSku";


        public static class StockClassification
        {
            public const string StockClassificationCode = "stockClassification";
            public static class StockClassificationParams
            {
                public const string AStockCode = "aStock";
                public const string BStockCode = "bStock";

            }
        }
        public static class BodyShape
        {
            public const string Code = "bodyShape";
            public const string BodenCode = "boden";
            public const string SälenCode = "salen";
            public const string VarbergCode = "varberg";
            public const string MeloriaCode = "meloria";
        }
    }
    public static class Flags
    {
        public static class StockClassification
        {
            public const string GroupCode = "stockClassification";
            public static class VariantFlags
            { 
                public const string BStock = "bStock";
                public const string AStock = "aStock";
            }
        }
        public static class EcommerceManagerStatus
        {
            public const string GroupCode = "ecommerceManagerStatus";
            public static class ProductFlags
            {
                public const string ReadyForSetup = "productReadyForSetupEcom";
                public const string InProgress = "productInProgressEcom";                
                public const string ProductFinished = "productFinishedEcom";
            }
            public static class VariantFlags
            {
                public const string ReadyForSetup = "variantReadyForSetupEcom";
                public const string InProgress = "variantInProgressEcom";
                public const string ProductFinished = "variantFinishedEcom";
            }
        }

        public static class MediaStatus
        {
            public const string GroupCode = "mediaStatus";

            public static class ProductFlags
            {
                public const string ReadyForSetup = "productReadyForSetupMedia";
                public const string InProgress = "productInProgressMedia";                
                public const string ProductFinished = "productFinishedMedia";
            }

            public static class VariantFlags
            {
                public const string ReadyForSetup = "variantReadyForSetupMedia";
                public const string InProgress = "variantInProgressMedia";
                public const string VariantFinished = "variantFinished";
            }

        }
        public static class ProductDataSpecialistStatus
        {
            public const string GroupCode = "productDataSpecialistStatus";

            public static class ProductFlags
            {
                public const string ReadyForSetup = "productReadyforSetup";
                public const string InProgress = "productInProgress";                
                public const string ProductFinished = "productFinished";
            }

            public static class VariantFlags
            {
                public const string ReadyForSetup = "variantReadyForSetup";
                public const string InProgress = "variantInProgress";
                public const string VariantFinished = "variantFinished";
            }
        }

        public static class StockSync
        {
            public const string GroupCode = "StockSync";
            public static class VariantFlags
            {
                public const string ExcludeVariant = "ExcludeVariant";
            }
            public static class ProductFlags
            {
                public const string ExcludeProduct = "ExcludeProduct";
            }
        }
    }

    public static class Export
    {
        public static class Status
        {
            public const string Success = "Success";
            public const string AlreadyInProgress = "AlreadyInProgress";
            public const string NoData = "NoData";
            public const string Failed = "Failed";
            public const string CompletedInExportFeed = "CompletedInExportFeed";
        }
    }
}