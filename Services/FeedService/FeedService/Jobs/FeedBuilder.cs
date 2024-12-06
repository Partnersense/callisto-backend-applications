// ---------------------------------------------------------------------------
// This file contains the FeedBuilder class. It fetches a full feed from
// Norce, maps the feed to different markets and then builds a unified feed
// where a product contains the property values of all different markets in
// the format: "PropertyName_Market": <PropertyValue>. It then sends the file
// to an Azure Storage blob to be used by external systems.
// ---------------------------------------------------------------------------

using System.Diagnostics;
using System.Globalization;
using System.Text.Json;
using FeedService.Constants;
using FeedService.Domain.DTOs.External.DataFeedWatch;
using FeedService.Domain.Models;
using FeedService.Domain.Norce;
using FeedService.Domain.Validation;
using FeedService.Services.CultureConfigurationServices;
using FeedService.Services.CultureFeedGenerationServices;
using FeedService.Services.PriceFeedGenerationServices;
using FeedService.Services.SalesAreaConfigurationServices;
using Hangfire;
using Microsoft.Extensions.Configuration.AzureAppConfiguration;
using Microsoft.Extensions.Options;
using SharedLib.Clients.Norce;
using SharedLib.Constants;
using SharedLib.Constants.NorceConstants;
using SharedLib.Jobs;
using SharedLib.Options.Models;
using SharedLib.Services;
using SharedLib.Util;
using NorceConstants = SharedLib.Constants.NorceConstants.NorceConstants;

namespace FeedService.Jobs;

public class FeedBuilder(
    ILogger<FeedBuilder> logger, 
    IOptionsMonitor<NorceProductFeedModuleOptions> feedOptions, 
    IOptionsMonitor<NorceBaseModuleOptions> norceOptions, 
    IOptionsMonitor<BaseModuleOptions> baseOptions, 
    IConfigurationRefresher configurationRefresher, 
    INorceClient norceClient,
    ICultureConfigurationService cultureConfigurationSerivce,
    ISalesAreaConfigurationService salesAreaConfigurationService,
    ICultureFeedGenerationService cultureFeedGenerationService,
    IPriceFeedGenerationService priceFeedGenerationService,
    IStorageService storageService) : JobBase<FeedBuilder>(logger)
{
    private readonly ILogger<FeedBuilder> _logger = logger;

    public override async Task Execute(CancellationToken cancellationToken)
    {

        // I want the feed builder class to only include a execute method that calls other classes, i do not want logic in this class, its not as neat and easely tested.
        // I want each separate logic to be in its own class so that it is easier to maintain, and switch out to a newer class in the future with the same interface, error tracing and so forth.

        // I have implemented more extensive logs in the services I have created, see examples in CultureConfigurationServiceExtension the logs are long yes, but they are in a 
        // in a format to make it easier to filter and make dashboards in elastic later on, make sure to use traceId in all methods to make it easier to filter out logs for
        // differet runs, this to make error searching in prod a lot easier, see examples of error handling and error logs in the CultureConfigurationServiceExtension aswelll
        var traceId = Guid.NewGuid();

        await configurationRefresher.TryRefreshAsync(cancellationToken);
        var stopwatch = new Stopwatch();
        stopwatch.Start();


        // Get cultures and sales areas with filters.
        var cultures = await cultureConfigurationSerivce.GetCultureConfigurations(traceId);
        var salesAreas = await salesAreaConfigurationService.GetSalesAreaConfigurations(traceId);

        // TODO: Generate feed with culture attributes, does not include price logic (resuse as much code from the old MapToFeedProducts method as possible, refine if possible)
        var feedWithCulturesNotPrice = await cultureFeedGenerationService.GenerateFeedWithCultures(cultures, traceId);

        // TODO: Add PriceLogic to the cuture feed (the best way to do it is the question), all product price logic needs to be fetched per sales area to get best price per sales area (pricelist seeds are in saleare object and is enriched in the GetSalesAreaConfigurations method)
        var feedWithCulturesAndPrice = await priceFeedGenerationService.GenerateFeedWithPrices(feedWithCulturesNotPrice, traceId);

        // TODO: Upload to Azure Storage method, create this service prefeibly in the lib as this could be done very generlized


        // old legacy method
        await GenerateFeed(cultures, salesAreas, cancellationToken);
        stopwatch.Stop();

        _logger.LogDebug("Time for all feeds to be fetched and processed: {TimeInSeconds} seconds", stopwatch.Elapsed.TotalSeconds);
    }

    private async Task GenerateFeed(List<CultureConfiguration> cultures, List<SalesAreaConfiguration> salesAreas, CancellationToken cancellationToken)
    {
        var feeds = cultures.Select(market => new MarketFeed { Market = market.MarketCode, Products = [] }).ToList();

        // Stream Norce full feed and map to feed object
        await foreach (var product in norceClient.ProductFeed.StreamProductFeedAsync(feedOptions.CurrentValue.ChannelKey, null, cancellationToken))

            foreach (var feed in feeds)
            {
                if (product != null)
                    feed.Products.AddRange(MapToFeedProducts(product, feed.Market));
            }

        var containerExists = await storageService.CreateStorageContainer(cancellationToken);
        if (containerExists)
        {
            var products = CombineMarketProducts(feeds);

            if (!products.Any())
            {
                _logger.LogError("No products for any market was found to build the feed. Aborting job.");
                return;
            }

            // Upload file to blob storage
            using var outputStream = new MemoryStream();
            await JsonSerializer.SerializeAsync(outputStream, products, cancellationToken: cancellationToken);
            outputStream.Position = 0;

            var success = await storageService.UploadFile(outputStream, true, cancellationToken, FileConstants.Json);

            if (success)
                _logger.LogInformation("Successfully uploaded feed to blob storage.");
            else
                _logger.LogError("Error uploading feed to blob storage.");
        }
    }

    /// <summary>
    /// Maps a norce feed product to a list of generic product DTOs based on market. Products and variants are flattened out to multiple products (1 per variant).
    /// </summary>
    /// <param name="norceProduct"></param>
    /// <param name="market"></param>
    /// <returns></returns>
    private IEnumerable<GenericFeedProductDto> MapToFeedProducts(NorceFeedProductDto norceProduct, string market)
    {
        if (norceProduct.Variants is null || norceProduct.Variants.Count <= 0)
        {
            return [];
        }


        var retVal = new List<GenericFeedProductDto>();
        var (cultureCode, pricelistCode) = GetMarketValues(market);

        foreach (var variant in norceProduct.Variants)
        {
            if (!ProductAndVariantIsValid(norceProduct, variant, cultureCode, pricelistCode)) continue;

            var price = variant.Prices.Find(price => price.PriceListCode.Equals(pricelistCode));
            var names = variant.Names.Find(name => name.CultureCode.Equals(cultureCode));
            var category = norceProduct.PrimaryCategory?.Cultures.Find(culture => culture.CultureCode.Equals(cultureCode))?.FullName;

            var title = names?.Name;
            var description = norceProduct.Texts.Find(text => text.CultureCode.Equals(cultureCode))?.Description;
            var imageUrl = GetImageUrl(variant.PrimaryImage?.Key);
            var stock = GetOnHands(variant, market);

            var productLink = GetProductLink(market, names?.UniqueUrlName);

            var priceValue = price?.ValueIncVat
                .ToString(CultureInfo.InvariantCulture);
            var currency = price?.Currency;

            var flags = GetFlags(norceProduct, cultureCode);

            var parametrics = GetParametrics(norceProduct, cultureCode);

            var item = new GenericFeedProductDto
            {
                Id = variant.PartNo,
                Title = title,
                Description = description ?? "",
                ProductLink = productLink,
                AdditionalImages = GetAdditionalImages(variant),
                ImageLink = imageUrl,
                Price = priceValue,
                Currency = currency,
                Availability = GetAvailability((int)stock),
                Category = category,
                EanCode = variant.EanCode,
                Flags = flags,
                Parametrics = parametrics
            };

            retVal.Add(item);
        }

        return retVal;
    }

    /// <summary>
    /// Merges all the feed products into a single product, and postfixes the properties with a language code. <br/>
    /// e.g. property "Title" is replaced by "Title_SE" for the swedish title, "Title_DK" for the danish title and so forth.
    /// </summary>
    /// <param name="marketFeeds"></param>
    /// <returns>A List of dictionaries, where each dictionary represents a product.</returns>
    private static List<Dictionary<string, object>> CombineMarketProducts(IEnumerable<MarketFeed> marketFeeds)
    {
        // Group products by Id across all markets
        var combinedProducts = marketFeeds
            .SelectMany(marketFeed => marketFeed.Products.Select(product => new { product, marketFeed.Market }))
            .GroupBy(p => p.product.Id)
            .Select(group =>
            {
                var dictionary = new Dictionary<string, object>();
                foreach (var marketProduct in group)
                {
                    // Reflect over the properties of GenericFeedProductDto and combine them with a market postfix
                    var market = marketProduct.Market;
                    var product = marketProduct.product;

                    var properties = product.GetType().GetProperties();
                    foreach (var property in properties)
                    {
                        var value = property.GetValue(product);
                        var propertyNameWithMarket = property.Name + "_" + market;
                        dictionary[propertyNameWithMarket] = value;
                    }
                }

                return dictionary;
            }).ToList();

        return combinedProducts;
    }

    /// <summary>
    /// Translates a market to other market specific values.
    /// </summary>
    /// <param name="market"></param>
    /// <returns></returns>
    private static (string cultureCode, string pricelistCode) GetMarketValues(string market)
    {
        return market.ToUpperInvariant() switch
        {
            LanguageConstants.Market.USA => (LanguageConstants.CultureCode.USA, PriceListsConstants.PriceListCodes.USStandard),
            LanguageConstants.Market.Europe => (LanguageConstants.CultureCode.EU, PriceListsConstants.PriceListCodes.EuropeStandard),
            LanguageConstants.Market.Sweden => (LanguageConstants.CultureCode.Sweden, PriceListsConstants.PriceListCodes.SwedenStandard),

            _ => ("", "")
        };
    }

    /// <summary>
    /// Gets an availability string based on the stock value.
    /// </summary>
    /// <param name="stock"></param>
    /// <returns></returns>
    private static string GetAvailability(int stock)
    {
        return stock > 0 ? FeedConstants.Stock.InStock : FeedConstants.Stock.OutOfStock;
    }

    /// <summary>
    /// Build the image URL for the image in the Norce CDN
    /// </summary>
    /// <param name="imageKey"></param>
    /// <returns></returns>
    private string GetImageUrl(string? imageKey)
    {
        return string.IsNullOrWhiteSpace(imageKey)
            ? string.Empty
            : new Uri(new Uri(norceOptions.CurrentValue.CdnUrl), imageKey).ToString();
    }

    /// <summary>
    /// Dynamically maps all flags to a dictionary.
    /// </summary>
    /// <param name="norceProduct"></param>
    /// <param name="cultureCode"></param>
    /// <returns></returns>
    private static Dictionary<string, string> GetFlags(NorceFeedProductDto norceProduct, string cultureCode)
    {
        var flags = new Dictionary<string, string>();
        foreach (var flag in norceProduct.Flags)
        {
            var cultureValue = flag.Cultures.Find(culture => culture.CultureCode == cultureCode);

            if (string.IsNullOrWhiteSpace(flag.GroupCode) || string.IsNullOrWhiteSpace(flag.Code) || string.IsNullOrWhiteSpace(cultureValue?.Name))
                continue;
            
            flags[flag.GroupCode ?? flag.Code] = cultureValue.Name;
        }

        return flags;
    }

    /// <summary>
    /// Fetches all parametrics (product, variant) and use reflection to map these to a dictionary. <br/>
    /// All parametrics are mapped as a flat object and will not be nested.
    /// </summary>
    /// <param name="norceProduct"></param>
    /// <param name="cultureCode"></param>
    /// <returns></returns>
    private static Dictionary<string, string> GetParametrics(NorceFeedProductDto norceProduct, string cultureCode)
    {
        var parametrics = new Dictionary<string, string>();

        var productParametrics = norceProduct.Parametrics;
        var variantParametrics = norceProduct.Variants?.SelectMany(variant =>
            variant.VariantDefiningParametrics) ?? [];
        var additionalParametrics = norceProduct.Variants?.SelectMany(variant =>
            variant.AdditionalParametrics) ?? [];

        var combinedParametrics = new List<Parametric>();
        combinedParametrics.AddRange(productParametrics);
        combinedParametrics.AddRange(variantParametrics);
        combinedParametrics.AddRange(additionalParametrics);

        foreach (var parametric in combinedParametrics)
        {
            var cultureValue = parametric.Cultures.Find(culture => culture.CultureCode == cultureCode);
            if (cultureValue is null || parametric.Code is null) continue;

            var parametricValue = "";
            if (!string.IsNullOrWhiteSpace(cultureValue.Value?.ToString()))
            {
                parametricValue = cultureValue.Value.ToString();
            }

            if (!string.IsNullOrWhiteSpace(cultureValue.ListValue?.Name))
            {
                parametricValue = cultureValue.ListValue.Name;
            }

            if (cultureValue.MultipleValues?.Count > 0)
            {
                parametricValue = string.Join(",", cultureValue.MultipleValues
                    .Where(v => !string.IsNullOrWhiteSpace(v.Name))
                    .Select(v => v.Name));
            }

            if (!string.IsNullOrWhiteSpace(parametricValue))
            {
                parametrics[parametric.Code] = parametricValue;
            }
        }

        return parametrics;
    }

    private static List<string?> GetAdditionalImages(NorceFeedVariant norceFeedVariant)
    {
        return norceFeedVariant.Files
            ?.Where(file => !string.IsNullOrWhiteSpace(file.Url) && !string.IsNullOrWhiteSpace(file.MimeType) &&
                            file.MimeType.Contains("image")).Select(image => image.Url).ToList() ?? [];
    }

    /// <summary>
    /// Gets the OnHands value from the warehouse that corresponds to the market.
    /// </summary>
    /// <param name="norceFeedVariant"></param>
    /// <param name="market"></param>
    /// <returns></returns>
    private static double GetOnHands(NorceFeedVariant norceFeedVariant, string market)
    {
        var isBStock = norceFeedVariant.Flags.Any(flag => !string.IsNullOrWhiteSpace(flag.Code) && flag.Code.Equals(NorceConstants.Flags.StockClassification.VariantFlags.BStock));

        var warehouseCode = MarketUtil.GetWarehouseCodeByMarket(market, isBStock);
        var onHandsValue = norceFeedVariant.OnHands?.Find(onHands => onHands.Warehouse is not null && onHands.Warehouse.Code.Equals(warehouseCode));

        return onHandsValue?.Value ?? 0;
    }

    /// <summary>
    /// Builds a product link based on market.
    /// </summary>
    /// <param name="market"></param>
    /// <param name="productUrlName"></param>
    /// <returns></returns>
    private string GetProductLink(string market, string? productUrlName)
    {
        if (string.IsNullOrWhiteSpace(market)) return "";
        var marketSlug = market == LanguageConstants.Market.USA ? "" : $"{market.ToLower()}/";
        return $"{baseOptions.CurrentValue.SiteUrl}/{marketSlug}product/{productUrlName}";
    }

    private bool ProductAndVariantIsValid(NorceFeedProductDto product, NorceFeedVariant norceFeedVariant, string cultureCode, string pricelistCode)
    {
        var variantValidator = new VariantValidator(pricelistCode, cultureCode);
        var productValidator = new ProductValidator(cultureCode);

        var productValidationResult = productValidator.Validate(product);

        if (!productValidationResult.IsValid)
        {
            foreach (var error in productValidationResult.Errors)
            {
                _logger.LogDebug("Product with Code {Code}: {ErrorMessage}", product.Code, error.ErrorMessage);
            }

            return false;
        }

        var variantValidationResult = variantValidator.Validate(norceFeedVariant);

        if (!variantValidationResult.IsValid)
        {
            foreach (var error in variantValidationResult.Errors)
            {
                _logger.LogDebug("Variant with partNo {PartNo}: {ErrorMessage}", norceFeedVariant.PartNo, error.ErrorMessage);
            }

            return false;
        }

        return true;
    }
}
