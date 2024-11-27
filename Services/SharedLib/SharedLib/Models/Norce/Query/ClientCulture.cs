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
    ///     "CultureCode": "string",
    ///     "ClientId": 0,
    ///     "IsPrimary": true,
    ///     "Created": "2019-08-24T14:15:22Z",
    ///     "CreatedBy": 0,
    ///     "Updated": "2019-08-24T14:15:22Z",
    ///     "UpdatedBy": 0,
    ///     "Collation": "string"
    /// }
    /// </example>
    public class ClientCulture
    {
        /// <summary>
        /// CultureCode. MaxLength: 16. Key property.
        /// </summary>
        /// <example>string</example>
        [StringLength(16)]
        [Required]
        [JsonPropertyName("cultureCode")]
        public string CultureCode { get; init; } = string.Empty;

        /// <summary>
        /// ClientId.
        /// </summary>
        /// <example>0</example>
        [Required]
        [JsonPropertyName("clientId")]
        public int ClientId { get; init; }

        /// <summary>
        /// IsPrimary.
        /// </summary>
        /// <example>true</example>
        [Required]
        [JsonPropertyName("isPrimary")]
        public bool IsPrimary { get; init; }

        /// <summary>
        /// Created.
        /// </summary>
        /// <example>2019-08-24T14:15:22Z</example>
        [Required]
        [JsonPropertyName("created")]
        public DateTime Created { get; init; }

        /// <summary>
        /// CreatedBy.
        /// </summary>
        /// <example>0</example>
        [Required]
        [JsonPropertyName("createdBy")]
        public int CreatedBy { get; init; }

        /// <summary>
        /// Updated. Nullable.
        /// </summary>
        /// <example>2019-08-24T14:15:22Z</example>
        [JsonPropertyName("updated")]
        public DateTime? Updated { get; init; }

        /// <summary>
        /// UpdatedBy. Nullable.
        /// </summary>
        /// <example>0</example>
        [JsonPropertyName("updatedBy")]
        public int? UpdatedBy { get; init; }

        /// <summary>
        /// Collation. MaxLength: 128. Nullable.
        /// </summary>
        /// <example>string</example>
        [StringLength(128)]
        [JsonPropertyName("collation")]
        public string? Collation { get; init; }
    }
}
