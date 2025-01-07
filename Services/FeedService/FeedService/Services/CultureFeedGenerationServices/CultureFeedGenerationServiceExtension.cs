
using FeedService.Domain.DTOs.External.DataFeedWatch;
using FeedService.Domain.Models;
using FeedService.Domain.Norce;
using FeedService.Domain.Validation;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SharedLib.Constants;
using SharedLib.Logging.Enums;
using SharedLib.Options.Models;
using System.Xml.Linq;
using static FeedService.Constants.FeedConstants;
using static SharedLib.Constants.LanguageConstants;
using static SharedLib.Constants.NorceConstants.PriceListsConstants;
using static SharedLib.Constants.SageConstants.Fields;

namespace FeedService.Services.CultureFeedGenerationServices
{
    /// <summary>
    /// Extension methods for transforming Norce products into culture-specific feed data.
    /// Contains pure business logic for product mapping and transformation, excluding price and stock handling.
    /// </summary>
    public static class CultureFeedGenerationServiceExtension
    {
        /// <summary>
        /// Maps a Norce product to its culture-specific representation, excluding price and stock information.
        /// </summary>
        /// <param name="product">The Norce product to map</param>
        /// <param name="culture">Culture configuration to apply</param>
        /// <param name="norceOptions">Norce configuration options</param>
        /// <param name="logger">Logger instance</param>
        /// <param name="traceId">Unique identifier for request tracing</param>
        /// <returns>Culture-specific product representation or null if mapping fails</returns>
        /// <exception cref="ArgumentNullException">Thrown when product or culture is null</exception>
        public static List<DataFeedWatchDto>? MapProductForCulture(NorceFeedProductDto product, CultureConfiguration culture, IOptionsMonitor<NorceBaseModuleOptions> norceOptions, IOptionsMonitor<BaseModuleOptions> baseOptions, ILogger logger, Guid? traceId = null)
        {
            try
            {
                if (product == null) throw new ArgumentNullException(nameof(product));
                if (culture == null) throw new ArgumentNullException(nameof(culture));

                var processedProducts = new List<DataFeedWatchDto>();

                if (product.Variants == null || !product.Variants.Any())
                    return processedProducts;

                var cultureCode = culture.CultureCode;
                var cultureUrl = baseOptions.CurrentValue.CultureProductUrlList[cultureCode];

                foreach (var variant in product.Variants)
                {
                    if (!ProductAndVariantIsValid(product, variant, cultureCode, logger, traceId))
                        continue;

                    var names = variant.Names.Find(name => name.CultureCode.Equals(cultureCode));
                    var texts = product.Texts?.Find(text => text.CultureCode.Equals(cultureCode));
                    var category = product.PrimaryCategory?.Cultures.Find(c => c.CultureCode.Equals(cultureCode))?.FullName;

                    // Get variant-specific parametric values
                    var colorParametric = variant.VariantDefiningParametrics.FirstOrDefault(p => p.Code.Equals("color", StringComparison.OrdinalIgnoreCase));
                    var colorValue = GetCultureSpecificParametricValue(colorParametric, cultureCode);

                    var sizeParametric = variant.VariantDefiningParametrics.FirstOrDefault(p => p.Code.Equals("storlek", StringComparison.OrdinalIgnoreCase) || p.Code.Equals("size", StringComparison.OrdinalIgnoreCase));
                    var sizeValue = GetCultureSpecificParametricValue(sizeParametric, cultureCode);

                    //Get other Properties
                    var productLink = GetProductLink(cultureUrl, names?.UniqueUrlName);
                    var additionalImages = ExtractAdditionalImages(variant.Files);
                    var productFlags = ExtractFlags(product.Flags, cultureCode);
                    var variantFlags = ExtractFlags(variant.Flags, cultureCode);
                    var parametrics = ExtractParametrics(product, variant, cultureCode);

                    var item = new DataFeedWatchDto
                    {
                        // Core product identification
                        Id = variant.PartNo,
                        ManufacturerPartNo = variant.ManufacturerPartNo,
                        Title = names?.Name ?? "",
                        UniqueUrlName = names?.UniqueUrlName,
                        EanCode = variant.EanCode,
                        Status = variant.Status,
                        ProductFlags = productFlags,
                        VariantFlags = variantFlags,
                        ImageLink = variant.PrimaryImage?.Url,
                        AdditionalImages = additionalImages,
                        Size = sizeValue,
                        Color = colorValue,
                        Parametrics = parametrics,
                        Description = texts?.Description ?? "",
                        SubHeader = texts?.SubHeader,
                        SubDescription = texts?.SubDescription,
                        Width = variant.Logistics?.Width,
                        Height = variant.Logistics?.Height,
                        Depth = variant.Logistics?.Depth,
                        Weight = variant.Logistics?.Weight,
                        CommodityCode = variant.CommodityCode,
                        RecommendedQuantity = variant.RecommendedQty,
                        IsRecommendedQuantityFixed = variant.IsRecommendedQtyFixed,
                        StartDate = variant.StartDate,
                        EndDate = variant.EndDate,
                        Type = variant.Type,


                        ProductLink = productLink,
                        ManufacturerName = product.Manufacturer?.Name,
                        ManufacturerCode = product.Manufacturer?.Code,
                        Category = category,
                    };

                    processedProducts.Add(item);
                }

                logger.LogInformation(
                    "TraceId: {traceId} Service: {serviceName} LogType: {logType} Method: {method} Message: {message} | Other Parameters ProductCode: {productCode}, CultureCode: {cultureCode}, VariantCount: {variantCount}",
                    traceId,
                    nameof(CultureFeedGenerationServiceExtension),
                    nameof(LoggingTypes.CheckpointLog),
                    nameof(MapProductForCulture),
                    "Successfully mapped product variants for culture",
                    product.Code,
                    cultureCode,
                    processedProducts.Count
                );

                return processedProducts;
            }
            catch (Exception ex)
            {
                logger.LogError(
                    ex,
                    "TraceId: {traceId} Service: {serviceName} LogType: {logType} Method: {method} Error Source: {errorSource} Error Message: {errorMessage} Error Stacktrace: {errorStackTrace} Error Inner Exception: {errorInnerException} Internal Message: {internalMessage} | Other Parameters ProductCode: {productCode}, Culture: {culture}",
                    traceId,
                    nameof(CultureFeedGenerationServiceExtension),
                    nameof(LoggingTypes.ErrorLog),
                    nameof(MapProductForCulture),
                    ex.Source,
                    ex.Message,
                    ex.StackTrace,
                    ex.InnerException,
                    "Error mapping product for culture",
                    product?.Code ?? "unknown",
                    culture?.CultureCode ?? "unknown"
                );
                throw;
            }
        }

        /// <summary>
        /// Validates a product and its variant against culture and price list requirements.
        /// </summary>
        /// <param name="product">The Norce product to validate</param>
        /// <param name="norceFeedVariant">The variant to validate</param>
        /// <param name="cultureCode">Culture code for validation</param>
        /// <param name="pricelistCode">Price list code for validation</param>
        /// <param name="logger">Logger instance</param>
        /// <param name="traceId">Unique identifier for request tracing</param>
        /// <returns>True if both product and variant are valid, false otherwise</returns>
        /// <exception cref="ArgumentNullException">Thrown when required parameters are null</exception>
        public static bool ProductAndVariantIsValid(NorceFeedProductDto product, NorceFeedVariant norceFeedVariant, string cultureCode, ILogger logger, Guid? traceId = null)
        {
            try
            {
                // Validate input parameters
                if (product == null) throw new ArgumentNullException(nameof(product));
                if (norceFeedVariant == null) throw new ArgumentNullException(nameof(norceFeedVariant));
                if (string.IsNullOrEmpty(cultureCode)) throw new ArgumentException("Culture code cannot be empty", nameof(cultureCode));

                var variantValidator = new VariantValidator(cultureCode);
                var productValidator = new ProductValidator(cultureCode);

                // Validate product
                var productValidationResult = productValidator.Validate(product);
                if (!productValidationResult.IsValid)
                {
                    var errors = string.Join("; ", productValidationResult.Errors.Select(e => e.ErrorMessage));
                    logger.LogInformation(
                        "TraceId: {traceId} Service: {serviceName} LogType: {logType} Method: {method} Message: {message} | Other Parameters ProductCode: {productCode}, Errors: {errors}",
                        traceId,
                        nameof(CultureFeedGenerationServiceExtension),
                        nameof(LoggingTypes.IssueLog),
                        nameof(ProductAndVariantIsValid),
                        "Product validation failed",
                        product.Code,
                        errors
                    );
                    return false;
                }

                // Validate variant
                var variantValidationResult = variantValidator.Validate(norceFeedVariant);
                if (!variantValidationResult.IsValid)
                {
                    var errors = string.Join("; ", variantValidationResult.Errors.Select(e => e.ErrorMessage));
                    logger.LogInformation(
                        "TraceId: {traceId} Service: {serviceName} LogType: {logType} Method: {method} Message: {message} | Other Parameters VariantPartNo: {variantPartNo}, Errors: {errors}",
                        traceId,
                        nameof(CultureFeedGenerationServiceExtension),
                        nameof(LoggingTypes.IssueLog),
                        nameof(ProductAndVariantIsValid),
                        "Variant validation failed",
                        norceFeedVariant.PartNo,
                        errors
                    );
                    return false;
                }

                return true;
            }
            catch (Exception ex)
            {
                logger.LogError(
                    ex,
                    "TraceId: {traceId} Service: {serviceName} LogType: {logType} Method: {method} Error Source: {errorSource} Error Message: {errorMessage} Error Stacktrace: {errorStackTrace} Error Inner Exception: {errorInnerException} Internal Message: {internalMessage} | Other Parameters ProductCode: {productCode}, VariantPartNo: {variantPartNo}",
                    traceId,
                    nameof(CultureFeedGenerationServiceExtension),
                    nameof(LoggingTypes.ErrorLog),
                    nameof(ProductAndVariantIsValid),
                    ex.Source,
                    ex.Message,
                    ex.StackTrace,
                    ex.InnerException,
                    "Error during product and variant validation",
                    product?.Code ?? "unknown",
                    norceFeedVariant?.PartNo ?? "unknown"
                );
                throw;
            }
        }

        #region MappingMethods

        /// <summary>
        /// Gets the culture-specific value from a parametric.
        /// </summary>
        public static string? GetCultureSpecificParametricValue(Parametric? parametric, string cultureCode)
        {
            if (parametric == null) return null;

            var cultureValue = parametric.Cultures.FirstOrDefault(c => c.CultureCode == cultureCode);
            if (cultureValue?.ListValue != null)
            {
                return cultureValue.ListValue.Name;
            }
            return null;
        }

        /// <summary>
        /// Constructs the product link based on URL and unique url name.
        /// </summary>
        public static string GetProductLink(string url, string? uniqueUrlName)
        {
            if (string.IsNullOrWhiteSpace(uniqueUrlName) || string.IsNullOrEmpty(url)) return string.Empty;
            return url.Replace("uniqueUrlName", uniqueUrlName);
        }

        /// <summary>
        /// Extracts additional images from variant files.
        /// </summary>
        public static List<string> ExtractAdditionalImages(List<Domain.Norce.File>? files)
        {
            return files?
                .Where(f => f.Type == "asset" && !string.IsNullOrEmpty(f.Url))
                .Select(f => f.Url!)
                .ToList() ?? [];
        }

        /// <summary>
        /// Extracts flags with culture-specific values.
        /// </summary>
        public static Dictionary<string, string> ExtractFlags(List<Flag> flags, string cultureCode)
        {
            var flagsDictionary = new Dictionary<string, string>();
            foreach (var flag in flags)
            {
                var cultureValue = flag.Cultures.Find(culture => culture.CultureCode == cultureCode);

                if (string.IsNullOrWhiteSpace(flag.GroupCode) ||
                    string.IsNullOrWhiteSpace(flag.Code) ||
                    string.IsNullOrWhiteSpace(cultureValue?.Name))
                    continue;

                flagsDictionary[flag.GroupCode ?? flag.Code] = cultureValue.Name;
            }

            return flagsDictionary;
        }

        /// <summary>
        /// Extracts parametrics with culture-specific values.
        /// </summary>
        public static Dictionary<string, string> ExtractParametrics(NorceFeedProductDto product, NorceFeedVariant variant, string cultureCode)
        {
            var parametrics = new Dictionary<string, string>();

            // Combine all parametrics
            var allParametrics = new List<Parametric>();
            allParametrics.AddRange(product.Parametrics);
            allParametrics.AddRange(variant.VariantDefiningParametrics);
            allParametrics.AddRange(variant.AdditionalParametrics);

            foreach (var parametric in allParametrics)
            {
                if (string.IsNullOrEmpty(parametric.Code)) continue;

                var cultureValue = parametric.Cultures
                    .FirstOrDefault(c => c.CultureCode == cultureCode);

                if (cultureValue != null)
                {
                    string? value = null;

                    if (cultureValue.Value != null)
                    {
                        value = cultureValue.Value.ToString();
                    }
                    else if (cultureValue.ListValue != null)
                    {
                        value = cultureValue.ListValue.Name;
                    }
                    else if (cultureValue.MultipleValues?.Any() == true)
                    {
                        value = string.Join(",", cultureValue.MultipleValues.Select(v => v.Name));
                    }

                    if (!string.IsNullOrEmpty(value))
                    {
                        parametrics[parametric.Code] = value;
                    }
                }
            }

            return parametrics;
        }


        #endregion


    }
}