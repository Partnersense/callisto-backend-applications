using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using Enferno.Services.StormConnect.Contracts.Product.Models;
using Serilog;
using SharedLib.Constants;
using SharedLib.Constants.NorceConstants;
using SharedLib.Models.Sage;
using SharedLib.Util;

namespace SharedLib.Mappers;

public static class SageMapper
{
    /// <summary>
    /// Maps a Sage product from the provided query to a Norce <see cref="Product"/> instance, based on the specified group.
    /// </summary>
    /// <param name="group">The group identifier used to group the Norce variants in Products.</param>
    /// <param name="sageQuery">A list of <see cref="QueryLine"/> objects representing the Sage product query.</param>
    /// <returns>A <see cref="Product"/> instance mapped from the Sage query, or <c>null</c> if no product could be found.</returns>
    public static Product? MapSageProductToNorceProduct(string group, List<QueryLine> sageQuery)
    {
        var product = sageQuery.FirstOrDefault();

        if (product == null)
        {
            Log.Logger.Warning("Could not find a product to map.");
            return null;
        }

        var norceProduct = new Product
        {
            ManufacturerCode = NorceConstants.Manufacturer.Strandberg.Code,
            ManufacturerName = NorceConstants.Manufacturer.Strandberg.Name,
            Categories = new List<ProductCategory>
            {
                new()
                {
                    SortOrder = 0,
                    Code = CategoryConstants.ProductInstrumentCategoryCodes.Imported
                },
            },
            ProductFlags = GetProductFlags(),
            ProductParametrics = GetProductParametrics(product),
            Variants = GetNorceProductVariants(sageQuery),
        };

        return norceProduct;
    }

    /// <summary>
    /// Maps stock information from a Sage stock result to a Norce <see cref="SkuOnhand"/> object based on the provided warehouse code.
    /// </summary>
    /// <param name="sageStock">A <see cref="SageStockResultLine"/> object containing the stock information from Sage.</param>
    /// <param name="warehouseCode">The code of the warehouse to map to Norce.</param>
    /// <returns>A <see cref="SkuOnhand"/> object with the mapped stock information, or <c>null</c> if required fields are missing or invalid.</returns>

    public static SkuOnhand? MapSageStockToNorceOnhands(SageStockResultLine sageStock, string warehouseCode)
    {
        var partNo = sageStock.Fields.GetFieldByName(SageConstants.Fields.StockField.PartNo);
        var stockQuantityString = sageStock.Fields.GetFieldByName(SageConstants.Fields.StockField.Quantity);

        if (decimal.TryParse(stockQuantityString, out var onHandValue) && !string.IsNullOrEmpty(partNo))
        {
            var warehouse = SageToNorceTranslator.SageWarehouseToNorceWarehouse(warehouseCode);
            return new SkuOnhand
            {
                LocationCode = warehouse.locationCode,
                WarehouseCode = warehouse.warehouseCode,
                PartNo = partNo,
                OnhandValue = onHandValue
            };
        }

        var sb = new StringBuilder("Missing fields: ");
        if (string.IsNullOrEmpty(partNo)) sb.Append("PartNo ");
        if (string.IsNullOrEmpty(stockQuantityString)) sb.Append("OnhandValue ");

        Log.Logger.Warning(sb.ToString());
        return null;
    }

    /// <summary>
    /// Maps price information from a Sage price query to a Norce <see cref="SkuPriceList"/> object.
    /// </summary>
    /// <param name="sageQuery">A <see cref="QueryLine"/> object containing price information from Sage.</param>
    /// <returns>A <see cref="SkuPriceList"/> object with the mapped price details, or <c>null</c> if required fields are missing or invalid.</returns>

    public static SkuPriceList? MapSagePriceToNorceSkuPriceList(QueryLine sageQuery)
    {
        var partNo = sageQuery.Fields.GetFieldByName(SageConstants.Fields.PriceField.PartNo) ?? "";
        var priceString = sageQuery.Fields.GetFieldByName(SageConstants.Fields.PriceField.Price) ?? "";
        var currencyCode = sageQuery.Fields.GetFieldByName(SageConstants.Fields.PriceField.CurrencyCode) ?? "";
        var priceListCode = SageToNorceTranslator.SageCurrencyCodeToNorcePriceListCode(currencyCode);

        if (!AreValuesValid([partNo, priceString, currencyCode, priceListCode]))
        {
            Log.Logger.Warning("Could not find required values for pricelist - PartNo: {PartNo}, Price: {Price}, Currency: {Currency}, PricelistCode: {PricelistCode}", partNo, priceString, currencyCode, priceListCode);
            return null;
        }

        if (decimal.TryParse(priceString, NumberStyles.Any, CultureInfo.InvariantCulture, out var price) && !string.IsNullOrEmpty(partNo))
        {
            return new SkuPriceList
            {
                PartNo = partNo,
                PriceListCode = priceListCode,
                PriceSale = price,
                CurrencyCode = currencyCode
            };
        }

        return null;
    }

    /// <summary>
    /// Generates a list of Norce <see cref="Variant"/> objects from the provided Sage query items.
    /// </summary>
    /// <param name="sageQuery">A list of <see cref="QueryLine"/> objects representing the Sage product items.</param>
    /// <returns>A list of <see cref="Variant"/> objects derived from the Sage product data.</returns>
    public static List<Variant> GetNorceProductVariants(List<QueryLine> sageQuery)
    {
        var variants = new List<Variant>();
        foreach (var item in sageQuery)
        {
            var partNo = item.Fields.GetFieldByName(SageConstants.Fields.ProductField.PartNo);
            Log.Logger.Information("Starting to map item {@partNo}", partNo);
            if (string.IsNullOrWhiteSpace(partNo))
            {
                Log.Logger.Warning("item {@Item} misses PartNo and will not be imported", item);
                continue;
            }

            var variantName = item.Fields.GetFieldByName(SageConstants.Fields.ProductField.DescriptionEn) ?? "";

            var statCodes = item.Fields.GetFieldByName(SageConstants.Fields.ProductField.StatusCodes) ?? "";
            var statCodeList = statCodes.Split(";");

            var csvData = item.Fields.GetFieldByName(SageConstants.Fields.ProductField.CSVdata) ?? "";
            var csvDataList = csvData.Split(";");

            var category = statCodeList[0];
            var isInstrument = IsInstrument(category);

            var variant = new Variant
            {
                ManufacturerPartNo = item.Fields.GetFieldByName(SageConstants.Fields.ProductField.PartNo),

                Cultures = new List<VariantCulture>
                {
                    new()
                    {
                        CultureCode = LanguageConstants.CultureCode.USA,
                        Name = variantName,
                    }
                },
                Skus = new List<Sku>
                {
                    new()
                    {
                        PartNo = partNo,
                        Status = (SkuStatus)2,
                        Type = (SkuType)1,
                        EanCode = null,
                        Cultures = new List<SkuCulture>()
                        {
                            new()
                            {
                                CultureCode = LanguageConstants.CultureCode.USA,
                                ErpName = $"Erp name - {variantName}"
                            }
                        },
                    }
                },
                VariantFlags = GetVariantFlags(isInstrument, partNo),
                VariantParametrics = GetVariantParametrics(item, partNo, statCodeList, csvDataList),
            };
            Log.Logger.Information("Mapped variant with ManufacturerPartNo {@ManufacturerPartNo}",variant.ManufacturerPartNo);
            Log.Logger.Information("Amount of Skus {@SkuCount}",variant.Skus.Count);
            Log.Logger.Information("Mapped Sku with ManufacturerPartNo {@SkuPartNo}",variant.Skus[0].PartNo);



            variants.Add(variant);
        }

        return variants;
    }

    /// <summary>
    /// Retrieves a list of <see cref="ParametricValue"/>s for a Norce import <see cref="Product"/>.
    /// </summary>
    /// <param name="product">The SageX3 product (<see cref="QueryLine"/>) to map from.</param>
    /// <returns>A list of <see cref="ParametricValue"/>.</returns>
    public static List<ParametricValue> GetProductParametrics(QueryLine product)
    {
        var parametrics = new List<ParametricValue>();

        var supplierName = product.Fields.GetFieldByName(SageConstants.Fields.ProductField.SupplierName);
        if (supplierName is not null)
        {
            var supplierNameParametricValues =
                NorceMapper.ConstructParametricListValue(NorceConstants.Parametrics.Supplier, FormatUtil.ToCamelCase(supplierName), LanguageConstants.CultureCode.USA, supplierName);

            parametrics.Add(supplierNameParametricValues);
        }

        return parametrics;
    }

    /// <summary>
    /// Retrieves a list of <see cref="Flag"/> objects representing the product flags.
    /// </summary>
    public static List<Flag> GetProductFlags()
    {
        // The first three flags are tested in order in unit tests. Add more flags after these to not break tests.
        var flags = new List<Flag>
        {
            new()
            {
                Code = NorceConstants.Flags.ProductDataSpecialistStatus.ProductFlags.ReadyForSetup,
                IsSet = true,
                IsLimitedUpdate = true,
            },
            new()
            {
                Code = NorceConstants.Flags.EcommerceManagerStatus.ProductFlags.ReadyForSetup,
                IsSet = true,
                IsLimitedUpdate = true,
            },
            new()
            {
                Code = NorceConstants.Flags.MediaStatus.ProductFlags.ReadyForSetup,
                IsSet = true,
                IsLimitedUpdate = true,
            }
        };

        return flags;
    }

    /// <summary>
    /// Retrieves a list of <see cref="Flag"/> objects representing the product flags.
    /// </summary>
    public static List<Flag> GetVariantFlags(bool isInstrument, string partNo)
    {
        // The first three flags are tested in order in unit tests. Add more flags after these to not break tests.
        var flags = new List<Flag>
        {
            new()
            {
                Code = NorceConstants.Flags.ProductDataSpecialistStatus.VariantFlags.ReadyForSetup,
                IsSet = true,
                IsLimitedUpdate = true,
            },
            new()
            {
                Code = NorceConstants.Flags.EcommerceManagerStatus.VariantFlags.ReadyForSetup,
                IsSet = true,
                IsLimitedUpdate = true,
            },
            new()
            {
                Code = NorceConstants.Flags.MediaStatus.VariantFlags.ReadyForSetup,
                IsSet = true,
                IsLimitedUpdate = true,
            }
        };

        if (isInstrument)
        {
            flags.Add(new Flag
                {
                    Code = IsBStock(partNo)
                        ? NorceConstants.Flags.StockClassification.VariantFlags.BStock
                        : NorceConstants.Flags.StockClassification.VariantFlags.AStock,
                    IsSet = true,
                }
            );
        }

        return flags;
    }

    /// <summary>
    /// Constructs a list of <see cref="ParametricValue"/> objects based on the provided <see cref="QueryLine"/> and associated data.
    /// </summary>
    /// <param name="item">A <see cref="QueryLine"/> object containing the fields from which parametric values are derived.</param>
    /// <param name="partNo">The part number associated with the variant.</param>
    /// <param name="statCodeList">An array of status codes used to determine various parametric values.</param>
    /// <param name="csvDataList">An array of CSV data values used to extract additional parametric values.</param>
    /// <returns>A list of <see cref="ParametricValue"/> objects representing various characteristics of the variant.</returns>
    public static List<ParametricValue> GetVariantParametrics(QueryLine item, string partNo, string[] statCodeList, string[] csvDataList)
    {
        var category = statCodeList[0];
        var parametrics = new List<ParametricValue>();

        var supplierPartNo = item.Fields.GetFieldByName(SageConstants.Fields.ProductField.SupplierPartNumber);
        if (supplierPartNo is not null)
        {
            var supplierNameParametricValues = NorceMapper.ConstructParametricTextValue(NorceConstants.Parametrics.SupplierPartNumber,
                supplierPartNo, LanguageConstants.CultureCode.USA);
            parametrics.Add(supplierNameParametricValues);
        }

        var artistSignatureProductValue = csvDataList[0];
        if (!string.IsNullOrWhiteSpace(artistSignatureProductValue))
        {
            var artistSignatureProduct = NorceMapper.ConstructParametricListValue(
                NorceConstants.Parametrics.ArtistSignatureProductCode, artistSignatureProductValue.ToLower(),
                LanguageConstants.CultureCode.USA);
            parametrics.Add(artistSignatureProduct);
        }

        var limitedEditionValue = csvDataList[1];
        if (!string.IsNullOrWhiteSpace(limitedEditionValue))
        {
            var limitedEdition = NorceMapper.ConstructParametricListValue(NorceConstants.Parametrics.LimitedEditionCode,
                limitedEditionValue.ToLower(), LanguageConstants.CultureCode.USA);
            parametrics.Add(limitedEdition);
        }

        if (IsInstrument(category))
        {
            var stockClassificationCode = IsBStock(partNo)
                ? NorceConstants.Parametrics.StockClassification.StockClassificationParams.BStockCode
                : NorceConstants.Parametrics.StockClassification.StockClassificationParams.AStockCode;

            var stockClassification = NorceMapper.ConstructParametricListValue(NorceConstants.Parametrics.StockClassification.StockClassificationCode,
                stockClassificationCode, LanguageConstants.CultureCode.USA);
            parametrics.Add(stockClassification);

            var instrumentProductionSku = ExtractInstrumentProductionSku(partNo);
            if (instrumentProductionSku != null) parametrics.Add(instrumentProductionSku);

            var yearModelValue = ExtractYearModel(partNo);
            if (yearModelValue is not null)
            {
                var yearModel = NorceMapper.ConstructParametricTextValue(NorceConstants.Parametrics.InstrumentYearModel,
                    yearModelValue, LanguageConstants.CultureCode.USA);
                parametrics.Add(yearModel);
            }
        }

        return parametrics;
    }

    private static bool IsInstrument(string statusCode)
    {
        return statusCode switch
        {
            SageConstants.Fields.ProductField.StatusCode.Bass
                or SageConstants.Fields.ProductField.StatusCode.SignatureBass
                or SageConstants.Fields.ProductField.StatusCode.Guitar
                or SageConstants.Fields.ProductField.StatusCode.SignatureGuitar => true,
            _ => false
        };
    }

    private static bool AreValuesValid(List<string> values) => values.TrueForAll(value => !string.IsNullOrWhiteSpace(value));

    /// <summary>
    /// Determines whether the given part number indicates a B-stock item based on its partNo suffix.
    /// </summary>
    /// <param name="partNo">The part number to check.</param>
    /// <returns><c>true</c> if the part number indicates B-stock; otherwise, <c>false</c>.</returns>
    private static bool IsBStock(string partNo)
    {
        var productRef = GetProductReference(partNo);
        return productRef.EndsWith("B");
    }

    private static string GetProductReference(string input)
    {
        if (string.IsNullOrEmpty(input))
            return input;

        var parts = input.Split('-');
        if (parts.Length < 3)
            return input;

        return string.Join("-", parts.Take(2));
    }

    /// <summary>
    /// Extracts the year model from a given part number by analyzing its format and applying specific patterns.
    /// </summary>
    /// <param name="partNo">The part number from which to extract the year model. If <c>null</c> or empty, the method returns <c>null</c>.</param>
    /// <returns>The extracted year model as a string if a match is found; otherwise, <c>null</c>.</returns>
    public static string? ExtractYearModel(string? partNo)
    {
        if (string.IsNullOrEmpty(partNo)) return null;

        var parts = partNo.Split('-');

        if (parts.Length < 2)
        {
            return null;
        }

        var secondPart = parts[1];

        var immediateHandlingPatterns = new []
        {
            @"^(1[7-9]|[2-9][0-9])\S{1,3}$", // Match 17+ followed by 1 to 3 non-whitespace characters
            @"\S{2}R$",                      // Match any two characters followed by R
            @"\S{2}JA$"                      // Match any two characters followed by JA
        };

        var resultPattern = @"^(1[7-9]|[2-9][0-9])|R$|JA$";

        foreach (var pattern in immediateHandlingPatterns)
        {
            var match = Regex.Match(secondPart, pattern, RegexOptions.None, TimeSpan.FromMilliseconds(100));
            if (match.Success)
            {
                var result = Regex.Match(match.Value, resultPattern, RegexOptions.None, TimeSpan.FromMilliseconds(100));
                return result.Value;
            }
        }

        return null;
    }

    /// <summary>
    ///  Extracts the Instrument prodcution sku from a given part number by analyzing its format and
    /// applying specific patterns. to group a stock and b stock part numbers for later BI tasks
    /// </summary>
    /// <param name="partNo"></param>
    /// <returns>partnumber for a stock and the a stock part number for b stock articles</returns>
    public static ParametricValue? ExtractInstrumentProductionSku(string partNo)
    {
        var instrumentProductionSku = NorceMapper.ConstructParametricListValue(
            NorceConstants.Parametrics.InstrumentProductionSku,
            partNo, LanguageConstants.CultureCode.USA, partNo);
        
        //Override to set the A-stock variant as Production SKU if B-Stock variant
        if(IsBStock(partNo))
        {
            var bStockInstrumentProductionSku = BStockInstrumentProductionSku(partNo);

            instrumentProductionSku = NorceMapper.ConstructParametricListValue(
                NorceConstants.Parametrics.InstrumentProductionSku,
                bStockInstrumentProductionSku, LanguageConstants.CultureCode.USA, bStockInstrumentProductionSku);
        }

        return instrumentProductionSku;
    }

    
    private static string BStockInstrumentProductionSku(string partNo)
    {
        // Split the part number into parts
        var parts = partNo.Split('-');
        
        // Identify the second-to-last part and strip the last 'B' if it ends with 'B'
        if (parts.Length >= 2 && parts[1].EndsWith("B"))
        {
            parts[1] = parts[1].TrimEnd('B');
        }
        
        // Reconstruct the part number
        var bStockInstrumentProductionSku = string.Join("-", parts);
        return bStockInstrumentProductionSku;
    }
}
