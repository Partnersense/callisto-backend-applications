using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace SharedLib.Models.Norce.Query
{
    /// <summary>
    /// Represents a client culture from the Norce API.
    /// GET /Application/ClientCultures
    /// </summary>
    /// <example>
    /// {
    ///      "CultureCode": "en-GB",
    ///      "ClientId": 1006,
    ///      "IsPrimary": false,
    ///      "Created": "2024-10-17T08:12:10.477Z",
    ///      "CreatedBy": 1195,
    ///      "Updated": null,
    ///      "UpdatedBy": null,
    ///      "Collation": null
    /// }
    /// </example>
    public class ClientCulture
    {
        /// <summary>
        /// CultureCode. MaxLength: 16. Key property. 
        /// <br/><br/>
        /// Example Values:
        /// "en-US", 
        /// "nb-NO",
        /// "sv-SE"
        /// </summary>
        [StringLength(16)]
        [Required]
        [JsonPropertyName("cultureCode")]
        public string CultureCode { get; init; } = string.Empty;

        /// <summary>
        /// ClientId.
        /// <br/><br/>
        /// Example Values:
        /// "1006"
        /// </summary>
        [Required]
        [JsonPropertyName("clientId")]
        public int ClientId { get; init; }

        /// <summary>
        /// IsPrimary.
        /// <br/><br/>
        /// Example Values:
        /// "false"
        /// </summary>
        [Required]
        [JsonPropertyName("isPrimary")]
        public bool IsPrimary { get; init; }

        /// <summary>
        /// Created.
        /// <br/><br/>
        /// Example Values:
        /// "2024-10-17T08:12:10.477Z"
        /// </summary>
        [Required]
        [JsonPropertyName("created")]
        public DateTime Created { get; init; }

        /// <summary>
        /// CreatedBy.
        /// <br/><br/>
        /// Example Values:
        /// "1195"
        /// </summary>
        [Required]
        [JsonPropertyName("createdBy")]
        public int CreatedBy { get; init; }

        /// <summary>
        /// Updated. Nullable.
        /// <br/><br/>
        /// Example Values:
        /// "null"
        /// "2024-10-17T08:12:10.477Z"
        /// </summary>
        [JsonPropertyName("updated")]
        public DateTime? Updated { get; init; }

        /// <summary>
        /// UpdatedBy. Nullable.
        /// <br/><br/>
        /// Example Values:
        /// "null"
        /// "1195"
        /// </summary>
        [JsonPropertyName("updatedBy")]
        public int? UpdatedBy { get; init; }

        /// <summary>
        /// Collation. MaxLength: 128. Nullable.
        /// <br/><br/>
        /// Example Values:
        /// </summary>
        [StringLength(128)]
        [JsonPropertyName("collation")]
        public string? Collation { get; init; }
    }
}
