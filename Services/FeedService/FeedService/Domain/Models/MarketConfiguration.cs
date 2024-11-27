using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace FeedService.Domain.Models
{
    /// <summary>
    /// Represents the configuration for a specific market, including culture and pricing information.
    /// This configuration is used to determine how products are displayed and priced in different regions.
    /// </summary>
    /// <example>
    /// var config = new MarketConfiguration 
    /// {
    ///     MarketCode = "US",
    ///     CultureCode = "en-US",
    ///     PriceListCode = "usStandard",
    ///     CurrencyCode = "USD"
    /// };
    /// </example>
    public class MarketConfiguration
    {
        /// <summary>
        /// The unique identifier code for the market (e.g., "US", "EU", "SE").
        /// This code is used to identify the specific regional market throughout the application.
        /// </summary>
        /// <example>US</example>
        [Required]
        [StringLength(10, MinimumLength = 2, ErrorMessage = "Market code must be between 2 and 10 characters")]
        [RegularExpression(@"^[A-Z]{2,10}$", ErrorMessage = "Market code must consist of uppercase letters only")]
        public required string MarketCode { get; init; }

        /// <summary>
        /// The culture/locale code for the market (e.g., "en-US", "sv-SE").
        /// Used for localization and formatting of text, numbers, and dates.
        /// </summary>
        /// <example>en-US</example>
        [Required]
        [StringLength(10, MinimumLength = 2, ErrorMessage = "Culture code must be between 2 and 10 characters")]
        [RegularExpression(@"^[a-z]{2}-[A-Z]{2}$", ErrorMessage = "Culture code must be in format 'xx-XX'")]
        public required string CultureCode { get; init; }

        /// <summary>
        /// The identifier for the price list associated with this market.
        /// Determines which pricing structure to use for products in this market.
        /// </summary>
        /// <example>usStandard</example>
        [Required]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Price list code must be between 3 and 50 characters")]
        public required string PriceListCode { get; init; }

        /// <summary>
        /// The ISO currency code for this market (e.g., "USD", "EUR").
        /// Used for monetary calculations and display.
        /// </summary>
        /// <example>USD</example>
        [Required]
        [StringLength(3, MinimumLength = 3, ErrorMessage = "Currency code must be exactly 3 characters")]
        [RegularExpression(@"^[A-Z]{3}$", ErrorMessage = "Currency code must be 3 uppercase letters")]
        public required string CurrencyCode { get; init; }

    }
}
