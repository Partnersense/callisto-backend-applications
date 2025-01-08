using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace SharedLib.Models.Norce.Api
{
    public class ApiItemWrapperResponse<T>
    {
        /// <summary>
        /// The amount of total items
        /// </summary>
        public int? ItemCount { get; set; }

        /// <summary>
        /// The list of Items
        /// </summary>
        public List<T>? Items { get; set; }
    }
}
