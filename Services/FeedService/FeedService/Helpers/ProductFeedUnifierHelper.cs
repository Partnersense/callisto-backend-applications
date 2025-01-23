using FeedService.Domain.Models;
using SharedLib.Logging.Enums;

namespace FeedService.Helpers
{
    public static class ProductFeedUnifierHelper
    {

        /// <summary>
        /// Combines products from different cultures and sales areas into unified products with market-specific properties.
        /// Properties are postfixed with culture code (e.g., Title_EN-US) or sales area code (e.g., Price_SE).
        /// Products with the same part number are combined into a single product.
        /// </summary>
        /// <param name="salesAreaConfigurations">List of sales area configurations with price information</param>
        /// <param name="cultureConfigurations">List of culture configurations with product information</param>
        /// <param name="logger">Logger instance for tracking method execution</param>
        /// <param name="traceId">Unique identifier for request tracing</param>
        /// <returns>A list of dictionaries where each dictionary represents a unified product with market-specific properties</returns>
        /// <exception cref="ArgumentNullException">Thrown when either configuration list is null</exception>
        public static List<Dictionary<string, object>> CombineMarketProducts(List<SalesAreaConfiguration> salesAreaConfigurations, List<CultureConfiguration> cultureConfigurations, ILogger logger, Guid? traceId = null)
        {
            try
            {
                if (salesAreaConfigurations == null) throw new ArgumentNullException(nameof(salesAreaConfigurations));
                if (cultureConfigurations == null) throw new ArgumentNullException(nameof(cultureConfigurations));
                if(!salesAreaConfigurations.Any() && !cultureConfigurations.Any()) throw new ArgumentException($"{nameof(salesAreaConfigurations)} and {nameof(cultureConfigurations)} can not both be empty");

                logger.LogInformation(
                    "TraceId: {traceId} Service: {serviceName} LogType: {logType} Method: {method} Message: {message} | Other Parameter Action: {action}",
                    traceId,
                    nameof(ProductFeedUnifierHelper),
                    nameof(LoggingTypes.CheckpointLog),
                    nameof(CombineMarketProducts),
                    "Starting product combination process",
                    nameof(MethodActionLogTypes.Starting)
                );

                // Create a dictionary to store products by part number
                var productsByPartNo = new Dictionary<string, Dictionary<string, object>>();

                // Process culture-specific products
                foreach (var culture in cultureConfigurations)
                {
                    ProcessCultureProducts(culture, productsByPartNo);
                }

                // Process sales area-specific products
                foreach (var salesArea in salesAreaConfigurations)
                {
                    ProcessSalesAreaProducts(salesArea, productsByPartNo);
                }

                var combinedProducts = productsByPartNo.Values.ToList();

                logger.LogInformation(
                    "TraceId: {traceId} Service: {serviceName} LogType: {logType} Method: {method} Message: {message} | Other Parameters ProductCount: {productCount}",
                    traceId,
                    nameof(ProductFeedUnifierHelper),
                    nameof(LoggingTypes.CheckpointLog),
                    nameof(CombineMarketProducts),
                    "Successfully combined products",
                    combinedProducts.Count
                );

                return combinedProducts;
            }
            catch (Exception ex)
            {
                logger.LogError(
                    ex,
                    "TraceId: {traceId} Service: {serviceName} LogType: {logType} Method: {method} Error Source: {errorSource} Error Message: {errorMessage} Error Stacktrace: {errorStackTrace} Error Inner Exception: {errorInnerException} Internal Message: {internalMessage}",
                    traceId,
                    nameof(ProductFeedUnifierHelper),
                    nameof(LoggingTypes.ErrorLog),
                    nameof(CombineMarketProducts),
                    ex.Source,
                    ex.Message,
                    ex.StackTrace,
                    ex.InnerException,
                    "Failed to combine market products"
                );
                throw;
            }
        }

        /// <summary>
        /// Processes culture-specific products and adds them to the combined products dictionary
        /// </summary>
        public static void ProcessCultureProducts(CultureConfiguration culture, Dictionary<string, Dictionary<string, object>> productsByPartNo)
        {
            foreach (var product in culture.Products)
            {
                if (!productsByPartNo.ContainsKey(product.Id))
                {
                    productsByPartNo[product.Id] = new Dictionary<string, object>();
                }

                var combinedProduct = productsByPartNo[product.Id];
                AddPropertiesWithSuffix(product, combinedProduct, culture.CultureCode);
            }
        }

        /// <summary>
        /// Processes sales area-specific products and adds them to the combined products dictionary
        /// </summary>
        public static void ProcessSalesAreaProducts(SalesAreaConfiguration salesArea, Dictionary<string, Dictionary<string, object>> productsByPartNo)
        {
            foreach (var priceProduct in salesArea.ProductsPriceInfo)
            {
                if (!productsByPartNo.ContainsKey(priceProduct.PartNo))
                {
                    productsByPartNo[priceProduct.PartNo] = new Dictionary<string, object>();
                }

                var combinedProduct = productsByPartNo[priceProduct.PartNo];
                AddPropertiesWithSuffix(priceProduct, combinedProduct, salesArea.SalesAreaCode ?? salesArea.SalesAreaId.ToString());
            }
        }

        /// <summary>
        /// Adds properties from a source object to a dictionary with appropriate suffix
        /// </summary>
        public static void AddPropertiesWithSuffix(
            object source,
            Dictionary<string, object> destination,
            string suffix)
        {
            var properties = source.GetType().GetProperties();
            foreach (var property in properties)
            {
                var value = property.GetValue(source);
                if (value != null)
                {
                    var propertyNameWithSuffix = $"{property.Name}_{suffix}";
                    destination[propertyNameWithSuffix] = value;
                }
            }
        }
    }
}

