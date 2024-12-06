using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedLib.Models.Norce.Query
{
    /// <summary>
    /// Represents a Currency from the Core namespace of NORCE Commerce Query API.
    /// Schema: Enferno.Storm.Query.Api.DataContexts.StormCore.Currency
    /// <br/><br/>
    /// Example Values:
    /// Id: 1
    /// Code: "EUR"
    /// DefaultName: "Euro"
    /// ExchangeRate: 1.0000000000
    /// DefaultPrefix: null
    /// DefaultSuffix: " €"
    /// IsActive: true
    /// Created: "2008-05-19T19:39:15.5Z"
    /// CreatedBy: 1
    /// Updated: null
    /// UpdatedBy: null
    /// </summary>
    public class CurrenciesResponse
    {
        /// <summary>
        /// Id. Key property.
        /// <br/><br/>
        /// Example Value: 1
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Code. MaxLength: 3.
        /// <br/><br/>
        /// Example Value: "EUR"
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// DefaultName. MaxLength: 50.
        /// <br/><br/>
        /// Example Value: "Euro"
        /// </summary>
        public string? DefaultName { get; set; }

        /// <summary>
        /// ExchangeRate.
        /// <br/><br/>
        /// Example Value: 1.0000000000
        /// </summary>
        public decimal ExchangeRate { get; set; }

        /// <summary>
        /// DefaultPrefix. MaxLength: 10.
        /// <br/><br/>
        /// Example Value: null
        /// </summary>
        public string? DefaultPrefix { get; set; }

        /// <summary>
        /// DefaultSuffix. MaxLength: 10.
        /// <br/><br/>
        /// Example Value: " €"
        /// </summary>
        public string? DefaultSuffix { get; set; }

        /// <summary>
        /// IsActive.
        /// <br/><br/>
        /// Example Value: true
        /// </summary>
        public bool IsActive { get; set; }

        /// <summary>
        /// Created.
        /// <br/><br/>
        /// Example Value: "2008-05-19T19:39:15.5Z"
        /// </summary>
        public DateTime Created { get; set; }

        /// <summary>
        /// CreatedBy.
        /// <br/><br/>
        /// Example Value: 1
        /// </summary>
        public int CreatedBy { get; set; }

        /// <summary>
        /// Updated.
        /// <br/><br/>
        /// Example Value: null
        /// </summary>
        public DateTime? Updated { get; set; }

        /// <summary>
        /// UpdatedBy.
        /// <br/><br/>
        /// Example Value: null
        /// </summary>
        public int? UpdatedBy { get; set; }

        ///// <summary>
        ///// Collection of currency cultures.
        ///// <br/><br/>
        ///// Example Value: not provided in example
        ///// </summary>
        //public ICollection<CurrencyCulture> Cultures { get; set; } = new List<CurrencyCulture>();
    }

    ///// <summary>
    ///// Represents a Currency Culture from the Core namespace of NORCE Commerce Query API.
    ///// Schema: Enferno.Storm.Query.Api.DataContexts.StormCore.CurrencyCulture
    ///// <br/><br/>
    ///// Example Values: Not provided in example
    ///// </summary>
    //public class CurrencyCulture
    //{
    //    /// <summary>
    //    /// CurrencyId. Key property.
    //    /// </summary>
    //    public int CurrencyId { get; set; }

    //    /// <summary>
    //    /// CultureCode. MaxLength: 16. Key property.
    //    /// </summary>
    //    public string CultureCode { get; set; } = null!;

    //    /// <summary>
    //    /// Name. MaxLength: 50.
    //    /// </summary>
    //    public string? Name { get; set; }

    //    /// <summary>
    //    /// Prefix. MaxLength: 10.
    //    /// </summary>
    //    public string? Prefix { get; set; }

    //    /// <summary>
    //    /// Suffix. MaxLength: 10.
    //    /// </summary>
    //    public string? Suffix { get; set; }

    //    /// <summary>
    //    /// Updated.
    //    /// </summary>
    //    public DateTime? Updated { get; set; }

    //    /// <summary>
    //    /// UpdatedBy.
    //    /// </summary>
    //    public int? UpdatedBy { get; set; }
    //}
}
