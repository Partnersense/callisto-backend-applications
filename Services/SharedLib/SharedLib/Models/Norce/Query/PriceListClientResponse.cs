
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedLib.Models.Norce.Query
{
    /// <summary>
    /// Represents a client price list configuration in the Norce Commerce system.
    /// Schema: Enferno.Storm.Query.Api.DataContexts.ApplicationModel.PriceListClient
    /// </summary>
    public class PriceListClientResponse
    {
        /// <summary>
        /// The ID of the price list.
        /// <br/><br/>
        /// Example Values:
        /// 1
        /// </summary>
        public int PriceListId { get; set; }

        /// <summary>
        /// The client ID associated with this price list.
        /// <br/><br/>
        /// Example Values:
        /// 1006
        /// </summary>
        public int ClientId { get; set; }

        /// <summary>
        /// The name of the price list.
        /// <br/><br/>
        /// Example Values:
        /// "Standard"
        /// </summary>
        public string? Name { get; set; }

        /// <summary>
        /// The description of the price list.
        /// <br/><br/>
        /// Example Values:
        /// null
        /// </summary>
        public string? Description { get; set; }

        /// <summary>
        /// The path to the image associated with this price list.
        /// <br/><br/>
        /// Example Values:
        /// null
        /// </summary>
        public string? ImagePath { get; set; }

        /// <summary>
        /// The agreement code for this price list.
        /// <br/><br/>
        /// Example Values:
        /// "STD"
        /// </summary>
        public string? Agreement { get; set; }

        /// <summary>
        /// The start date for this price list.
        /// <br/><br/>
        /// Example Values:
        /// null
        /// </summary>
        public DateTime? StartDate { get; set; }

        /// <summary>
        /// The end date for this price list.
        /// <br/><br/>
        /// Example Values:
        /// null
        /// </summary>
        public DateTime? EndDate { get; set; }

        /// <summary>
        /// The currency ID for this price list.
        /// <br/><br/>
        /// Example Values:
        /// 2
        /// </summary>
        public int CurrencyId { get; set; }

        /// <summary>
        /// The default price rule ID.
        /// <br/><br/>
        /// Example Values:
        /// 2
        /// </summary>
        public int? DefaultPriceRuleId { get; set; }

        /// <summary>
        /// The default price rule value.
        /// <br/><br/>
        /// Example Values:
        /// null
        /// </summary>
        public decimal? DefaultPriceRuleValue { get; set; }

        /// <summary>
        /// The default supplement charge percentage.
        /// <br/><br/>
        /// Example Values:
        /// 0.00
        /// </summary>
        public decimal DefaultSupplementChargePercentage { get; set; }

        /// <summary>
        /// The default kickback percentage.
        /// <br/><br/>
        /// Example Values:
        /// null
        /// </summary>
        public decimal? DefaultKickbackPercentage { get; set; }

        /// <summary>
        /// The default minimum product margin percentage.
        /// <br/><br/>
        /// Example Values:
        /// null
        /// </summary>
        public decimal? DefaultMinimumProductMarginPercentage { get; set; }

        /// <summary>
        /// The parent price list ID.
        /// <br/><br/>
        /// Example Values:
        /// null
        /// </summary>
        public int? ParentPriceListId { get; set; }

        /// <summary>
        /// The population rule ID.
        /// <br/><br/>
        /// Example Values:
        /// 1
        /// </summary>
        public int PopulationRuleId { get; set; }

        /// <summary>
        /// The chosen warehouse ID.
        /// <br/><br/>
        /// Example Values:
        /// 2
        /// </summary>
        public int? ChosenWarehouseId { get; set; }

        /// <summary>
        /// The chosen location ID.
        /// <br/><br/>
        /// Example Values:
        /// 2
        /// </summary>
        public int? ChosenLocationId { get; set; }

        /// <summary>
        /// Indicates if the price list is ERP updateable.
        /// <br/><br/>
        /// Example Values:
        /// false
        /// </summary>
        public bool IsErpUpdateable { get; set; }

        /// <summary>
        /// Indicates if the price list is ERP integrated.
        /// <br/><br/>
        /// Example Values:
        /// false
        /// </summary>
        public bool IsErpIntegrated { get; set; }

        /// <summary>
        /// Indicates if this is a primary price list.
        /// <br/><br/>
        /// Example Values:
        /// false
        /// </summary>
        public bool IsPrimary { get; set; }

        /// <summary>
        /// Indicates if this is a public price list.
        /// <br/><br/>
        /// Example Values:
        /// null
        /// </summary>
        public bool? IsPublic { get; set; }

        /// <summary>
        /// Indicates if the price list is limited to stock.
        /// <br/><br/>
        /// Example Values:
        /// null
        /// </summary>
        public bool? IsLimitedToStock { get; set; }

        /// <summary>
        /// Indicates if this is a bid price list.
        /// <br/><br/>
        /// Example Values:
        /// null
        /// </summary>
        public bool? IsBid { get; set; }

        /// <summary>
        /// Indicates if this is a Stako price list.
        /// <br/><br/>
        /// Example Values:
        /// null
        /// </summary>
        public bool? IsStako { get; set; }

        /// <summary>
        /// Indicates if this pricelist is active
        /// <br/><br/>
        /// </summary>
        public bool IsActive { get; set; }

        /// <summary>
        /// The creation date of the price list.
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
        /// The last update date of the price list.
        /// <br/><br/>
        /// Example Values:
        /// "2024-08-19T19:10:14.327Z"
        /// </summary>
        public DateTime? Updated { get; set; }

        /// <summary>
        /// The ID of the user who last updated the price list.
        /// <br/><br/>
        /// Example Values:
        /// 45
        /// </summary>
        public int? UpdatedBy { get; set; }

        /// <summary>
        /// The image key associated with this price list.
        /// <br/><br/>
        /// Example Values:
        /// null
        /// </summary>
        public Guid? ImageKey { get; set; }

        /// <summary>
        /// The type ID of the price list.
        /// <br/><br/>
        /// Example Values:
        /// 0
        /// </summary>
        public int TypeId { get; set; }

        /// <summary>
        /// The image extension.
        /// <br/><br/>
        /// Example Values:
        /// null
        /// </summary>
        public string? ImageExtension { get; set; }

        /// <summary>
        /// The default supplement charge.
        /// <br/><br/>
        /// Example Values:
        /// 0.0000
        /// </summary>
        public decimal? DefaultSupplementCharge { get; set; }

        /// <summary>
        /// Indicates if the price list is limited to recommended prices.
        /// <br/><br/>
        /// Example Values:
        /// null
        /// </summary>
        public bool? DefaultLimitToPriceRecommended { get; set; }

        /// <summary>
        /// Indicates if this is a virtual price list.
        /// <br/><br/>
        /// Example Values:
        /// false
        /// </summary>
        public bool IsVirtual { get; set; }

        /// <summary>
        /// The sales area ID.
        /// <br/><br/>
        /// Example Values:
        /// null
        /// </summary>
        public int? SalesAreaId { get; set; }

        /// <summary>
        /// The inherit structure calculation rule.
        /// <br/><br/>
        /// Example Values:
        /// 0
        /// </summary>
        public int InheritStructureCalculationRule { get; set; }

        /// <summary>
        /// The associated price list reference.
        /// <br/><br/>
        /// Example Values:
        /// null
        /// </summary>
        public PriceListResponse PriceList { get; set; }
    }
}
