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
    /// Represents a client sales area from the Norce API.
    /// GET /Application/SalesAreas
    /// </summary>
    /// <example>
    /// {
    ///      "SalesAreaId": 1,
    ///      "ClientId": 1006,
    ///      "IsPrimary": true,
    ///      "Created": "2024-02-20T15:30:01.477Z",
    ///      "CreatedBy": 9,
    ///      "Updated": "2024-08-20T11:21:16.15Z",
    ///      "UpdatedBy": 1035,
    ///      "Code": "SE"
    /// }
    /// </example>
    public class SalesAreaResponse
    {
        /// <summary>
        /// The unique identifier for the sales area. Key property.
        /// <br/><br/>
        /// Example Values:
        /// "1"
        /// </summary>
        [Required]
        [JsonPropertyName("salesAreaId")]
        public int SalesAreaId { get; init; }

        /// <summary>
        /// Client identifier.
        /// <br/><br/>
        /// Example Values:
        /// "1006"
        /// </summary>
        [Required]
        [JsonPropertyName("clientId")]
        public int ClientId { get; init; }

        /// <summary>
        /// Indicates if this is the primary sales area.
        /// <br/><br/>
        /// Example Values:
        /// "true"
        /// </summary>
        [Required]
        [JsonPropertyName("isPrimary")]
        public bool IsPrimary { get; init; }

        /// <summary>
        /// Creation timestamp.
        /// <br/><br/>
        /// Example Values:
        /// "2024-02-20T15:30:01.477Z"
        /// </summary>
        [Required]
        [JsonPropertyName("created")]
        public DateTime Created { get; init; }

        /// <summary>
        /// Identifier of the user who created the sales area.
        /// <br/><br/>
        /// Example Values:
        /// "9"
        /// </summary>
        [Required]
        [JsonPropertyName("createdBy")]
        public int CreatedBy { get; init; }

        /// <summary>
        /// Last update timestamp. Nullable.
        /// <br/><br/>
        /// Example Values:
        /// "null"
        /// "2024-08-20T11:21:16.15Z"
        /// </summary>
        [JsonPropertyName("updated")]
        public DateTime? Updated { get; init; }

        /// <summary>
        /// Identifier of the user who last updated the sales area. Nullable.
        /// <br/><br/>
        /// Example Values:
        /// "null"
        /// "1035"
        /// </summary>
        [JsonPropertyName("updatedBy")]
        public int? UpdatedBy { get; init; }

        /// <summary>
        /// The sales area code. MaxLength: 50. Nullable.
        /// <br/><br/>
        /// Example Values:
        /// "SE"
        /// </summary>
        [StringLength(50)]
        [JsonPropertyName("code")]
        public string? Code { get; init; }
    }
}

