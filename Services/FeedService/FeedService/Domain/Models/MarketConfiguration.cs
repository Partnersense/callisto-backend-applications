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

    }
}
