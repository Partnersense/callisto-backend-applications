using FeedService.Domain.Models;
using SharedLib.Logging.Enums;
using SharedLib.Models.Norce.Api;

namespace FeedService.Services.PriceFeedGenerationServices
{
    public static class PriceFeedGenerationServiceExtension
    {
        /// <summary>
        /// Maps ListProducts2Response objects to PriceProduct objects, skipping invalid products.
        /// </summary>
        /// <param name="products">List of products from the API response</param>
        /// <param name="logger">Logger instance</param>
        /// <param name="traceId">Unique identifier for request tracing</param>
        /// <returns>List of mapped PriceProduct objects</returns>
        /// <exception cref="ArgumentNullException">Thrown when products parameter is null</exception>
        public static List<PriceProduct> MapListProductsToPriceProducts(List<ListProducts2Response> products, ILogger logger, Guid? traceId = null)
        {
            try
            {
                if (products == null) throw new ArgumentNullException(nameof(products));

                var validProducts = new List<PriceProduct>();

                foreach (var product in products)
                {
                    if (!IsProductValid(product, logger, traceId))
                        continue;
                    

                    var mappedProduct = new PriceProduct
                    {
                        Id = product.Id,
                        PartNo = product.PartNo ?? string.Empty,
                        Name = product.Name ?? string.Empty,
                        Price = product.Price,
                        PriceIncVat = product.PriceIncVat,
                        PriceRecommended = product.PriceRecommended,
                        PriceCatalog = product.PriceCatalog,
                        VatRate = product.VatRate,
                        Stock = product.OnHand?.Value ?? 0,
                        IncomingStock = product.OnHand?.IncomingValue ?? 0,
                        NextDeliveryDate = product.OnHand?.NextDeliveryDate,
                        LeadTimeDayCount = product.OnHand?.LeadtimeDayCount,
                        IsStockActive = product.OnHand?.IsActive ?? false,
                        LastChecked = product.OnHand?.LastChecked,
                        IsReturnable = product.OnHand?.IsReturnable ?? false,
                        RecommendedQuantity = product.RecommendedQuantity,
                        IsRecommendedQuantityFixed = product.IsRecommendedQuantityFixed,
                        IsBuyable = product.IsBuyable,
                        Updated = product.Updated
                    };

                    validProducts.Add(mappedProduct);
                }

                return validProducts;
            }
            catch (Exception ex)
            {
                logger.LogError(
                    ex,
                    "TraceId: {traceId} Service: {serviceName} LogType: {logType} Method: {method} Error Source: {errorSource} Error Message: {errorMessage} Error Stacktrace: {errorStackTrace} Error Inner Exception: {errorInnerException} Internal Message: {internalMessage}",
                    traceId,
                    nameof(PriceFeedGenerationServiceExtension),
                    nameof(LoggingTypes.ErrorLog),
                    nameof(MapListProductsToPriceProducts),
                    ex.Source,
                    ex.Message,
                    ex.StackTrace,
                    ex.InnerException,
                    "Failed to map ListProducts2Response to PriceProduct"
                );
                throw;
            }
        }

        /// <summary>
        /// Validates a product for required fields and valid values.
        /// </summary>
        /// <param name="product">Product to validate</param>
        /// <param name="logger">Logger instance</param>
        /// <param name="traceId">Unique identifier for request tracing</param>
        /// <returns>True if the product is valid, false otherwise</returns>
        public static bool IsProductValid(ListProducts2Response product, ILogger logger, Guid? traceId)
        {
            var validationErrors = new List<string>();

            if (string.IsNullOrWhiteSpace(product.PartNo))
                validationErrors.Add("Missing PartNo");

            if (product.Price < 0)
                validationErrors.Add("Negative Price");

            if (product.PriceIncVat < 0)
                validationErrors.Add("Negative PriceIncVat");

            if (product.VatRate < 0)
                validationErrors.Add("Invalid VatRate");

            if (validationErrors.Any())
            {
                logger.LogInformation(
                    "TraceId: {traceId} Service: {serviceName} LogType: {logType} Method: {method} Message: {message} | Other Parameters ProductPartNo: {partNo}, ProductId {productId}, ValidationErrors: {errors}",
                    traceId,
                    nameof(PriceFeedGenerationServiceExtension),
                    nameof(LoggingTypes.IssueLog),
                    nameof(IsProductValid),
                    "Product validation failed",
                    product.PartNo ?? "No partNo",
                    product.Id,
                    string.Join(", ", validationErrors)
                );
                return false;
            }

            return true;
        }
    }
}

