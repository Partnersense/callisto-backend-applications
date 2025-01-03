using static SharedLib.Options.OptionsTypeConvertersExtension;
using System.ComponentModel;
using Microsoft.Extensions.Configuration;
using System.ComponentModel.DataAnnotations;

namespace SharedLib.Options.Models;

public class BaseModuleOptions
{
    private string _includedCulturesString = "";
    private string _includedSalesAreaIdsString = "";
    private string _cultureProductUrlString = "";

    /// <summary>
    /// The base site URL for the application
    /// </summary>
    [Required]
    public string SiteUrl { get; set; } = "";

    /// <summary>
    /// Raw string value from configuration
    /// Expected format: "en-US|sv-SE|nb-NO"
    /// </summary>
    public string IncludedCultures
    {
        get => _includedCulturesString;
        set
        {
            _includedCulturesString = value;
            IncludedCulturesList = OptionsTypeConvertersExtension.ConvertPipeSeparatedStringList(value);
        }
    }

    /// <summary>
    /// Raw string value from configuration
    /// Expected format: "en-US|sv-SE|nb-NO"
    /// </summary>
    public string IncludedSalesAreaIds
    {
        get => _includedSalesAreaIdsString;
        set
        {
            _includedSalesAreaIdsString = value;
            IncludedSalesAreaIdsList = OptionsTypeConvertersExtension.ConvertPipeSeparatedIntList(value);
        }
    }

    /// <summary>
    /// Raw string value from configuration
    /// Expected format: "en-US|sv-SE|nb-NO"
    /// </summary>
    public string CultureProductUrl
    {
        get => _cultureProductUrlString;
        set
        {
            _cultureProductUrlString = value;
            CultureProductUrlList = OptionsTypeConvertersExtension.ConvertPipeAndQuotesListToDictionary(value);
        }
    }


    /// <summary>
    /// List of culture codes to include in processing.
    /// Lazily converts from IncludedCulturesString if not already populated.
    /// </summary>
    public List<string> IncludedCulturesList { get; private set; } = [];

    /// <summary>
    /// List of sales area IDs to include in processing
    /// </summary>
    public List<int> IncludedSalesAreaIdsList {get; private set; } = [];

    /// <summary>
    /// List of sales area IDs to include in processing
    /// </summary>
    public Dictionary<string, string> CultureProductUrlList { get; private set; } = [];
}
