using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace SharedLib.Models.Norce.Query
{
    public class OdataWrapperResponse<T>
    {
        /// <summary>
        /// The OData context URL
        /// </summary>
        [JsonPropertyName("@odata.context")]
        public string? Context { get; set; }

        /// <summary>
        /// The actual data wrapped by the OData response
        /// </summary>
        [JsonPropertyName("value")]
        public List<T>? Value { get; set; }

        /// <summary>
        /// The link to the next page
        /// </summary>
        [JsonProperty(PropertyName = "@odata.nextLink")]
        public string Paging { get; set; }
    }
}
