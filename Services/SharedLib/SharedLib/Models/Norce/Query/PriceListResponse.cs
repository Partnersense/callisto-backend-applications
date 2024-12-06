using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedLib.Models.Norce.Query
{
    /// <summary>
    /// Represents a price list in the Norce Commerce system.
    /// Schema: Enferno.Storm.Query.Api.DataContexts.ApplicationModel.PriceList
    /// </summary>
    public class PriceListResponse
    {
        /// <summary>
        /// The unique identifier of the price list.
        /// <br/><br/>
        /// Example Values:
        /// 1
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// The default name of the price list.
        /// <br/><br/>
        /// Example Values:
        /// "Standard"
        /// </summary>
        public string? DefaultName { get; set; }

        /// <summary>
        /// The default description of the price list.
        /// <br/><br/>
        /// Example Values:
        /// null
        /// </summary>
        public string? DefaultDescription { get; set; }

        /// <summary>
        /// The agreement code associated with this price list.
        /// <br/><br/>
        /// Example Values:
        /// null
        /// </summary>
        public string? Agreement { get; set; }

        /// <summary>
        /// The start date of the price list validity period.
        /// <br/><br/>
        /// Example Values:
        /// null
        /// </summary>
        public DateTime? StartDate { get; set; }

        /// <summary>
        /// The end date of the price list validity period.
        /// <br/><br/>
        /// Example Values:
        /// null
        /// </summary>
        public DateTime? EndDate { get; set; }

        /// <summary>
        /// Indicates if the price list is public.
        /// <br/><br/>
        /// Example Values:
        /// false
        /// </summary>
        public bool IsPublic { get; set; }

        /// <summary>
        /// Indicates if the price list is limited to stock.
        /// <br/><br/>
        /// Example Values:
        /// false
        /// </summary>
        public bool IsLimitedToStock { get; set; }

        /// <summary>
        /// Indicates if this is a bid price list.
        /// <br/><br/>
        /// Example Values:
        /// false
        /// </summary>
        public bool IsBid { get; set; }

        /// <summary>
        /// Indicates if this is a Stako price list.
        /// <br/><br/>
        /// Example Values:
        /// false
        /// </summary>
        public bool IsStako { get; set; }

        /// <summary>
        /// Indicates if the price list is active.
        /// <br/><br/>
        /// Example Values:
        /// true
        /// </summary>
        public bool IsActive { get; set; }

        /// <summary>
        /// The date the price list was created.
        /// <br/><br/>
        /// Example Values:
        /// "2024-03-14T10:16:09.353Z"
        /// </summary>
        public DateTime Created { get; set; }

        /// <summary>
        /// The ID of the user who created the price list.
        /// <br/><br/>
        /// Example Values:
        /// 48
        /// </summary>
        public int CreatedBy { get; set; }

        /// <summary>
        /// The date the price list was last updated.
        /// <br/><br/>
        /// Example Values:
        /// null
        /// </summary>
        public DateTime? Updated { get; set; }

        /// <summary>
        /// The ID of the user who last updated the price list.
        /// <br/><br/>
        /// Example Values:
        /// null
        /// </summary>
        public int? UpdatedBy { get; set; }
    }
}
