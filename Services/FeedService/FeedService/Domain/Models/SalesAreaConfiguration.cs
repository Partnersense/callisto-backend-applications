using Enferno.Services.Contracts;
using System.ComponentModel.DataAnnotations;

namespace FeedService.Domain.Models
{
    /// <summary>
    /// Represents the configuration for a specific sales area, including regional and pricing settings.
    /// </summary>
    public class SalesAreaConfiguration
    {
        /// <summary>
        /// The unique identifier for the sales area.
        /// <br/><br/>
        /// Example Values:
        /// "1"
        /// </summary>
        [Required]
        public required int SalesAreaId { get; init; }

        /// <summary>
        /// The unique identifier code for the sales area (e.g., "SE", "US", "EU").
        /// This code is used to identify the specific regional sales area throughout the application.
        /// <br/><br/>
        /// Example Values:
        /// "null"
        /// "SE"
        /// "US"
        /// "EU"
        /// </summary>
        public string? SalesAreaCode { get; init; }

        /// <summary>
        /// The currency code for the sales area (e.g., "SEK", "USD", "EUR").
        /// Used for pricing and financial calculations within the sales area.
        /// <br/><br/>
        /// Example Values:
        /// "SEK"
        /// "USD"
        /// "EUR"
        /// </summary>
        public string? CurrencyCode { get; init; }

        /// <summary>
        /// The list of price list codes used for this sales area.
        /// Maps to the specific price list configurations in the system.
        /// <br/><br/>
        /// Example Values:
        /// ["SE-STANDARD", "SE-CAMPAIGN"]
        /// </summary>
        [Required]
        public required List<string> PriceListCodes { get; init; } = [];

        /// <summary>
        /// Indicates if this is the primary sales area for the region.
        /// Only one sales area per region should be marked as primary.
        /// <br/><br/>
        /// Example Values:
        /// "true"
        /// "false"
        /// </summary>
        [Required]
        public required bool IsPrimary { get; init; }

        /// <summary>
        /// The timestamp when this sales area configuration was created.
        /// <br/><br/>
        /// Example Values:
        /// "2024-02-20T15:30:01.477Z"
        /// </summary>
        [Required]
        public required DateTime Created { get; init; }

        /// <summary>
        /// The timestamp when this sales area configuration was last updated.
        /// <br/><br/>
        /// Example Values:
        /// "null"
        /// "2024-08-20T11:21:16.15Z"
        /// </summary>
        public DateTime? Updated { get; init; }
    }
}