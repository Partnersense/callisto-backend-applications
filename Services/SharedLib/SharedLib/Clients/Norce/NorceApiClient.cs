using System.Text.Json;
using SharedLib.Models.Norce.Api;
using Log = Serilog.Log;

namespace SharedLib.Clients.Norce;

public class NorceApiClient(INorceClient client)
{
    public async Task<string> GetAsync(string query, string? applicationId = null)
    {
        return await client.BaseGetAsync(query, applicationId);
    }

    public async Task<HttpResponseMessage> PostAsync(string query, string json, string? applicationId = null)
    {
        return await client.BasePostAsync(query, json, applicationId);
    }

    public async Task<string> GetApplication(string cultureCode)
    {
       return await client.BaseGetAsync(Endpoints.Api.Metadata.GetApplication(cultureCode));
    }

    /// <summary>
    /// Retrieves a product from the Norce API by its part number.
    /// </summary>
    /// <param name="partNo">The part number of the product to fetch from the Norce API.</param>
    /// <returns>
    /// Returns a <see cref="NorceApiProduct"/> object if the product is successfully retrieved and deserialized.
    /// If the product is not found or an error occurs, returns <c>null</c>.
    /// </returns>
    public async Task<NorceApiProduct?> GetProductByPartNo(string partNo)
    {
        var json = await client.BaseGetAsync(Endpoints.Api.Product.GetProductByPartNo(partNo));
        if (string.IsNullOrWhiteSpace(json))
        {
            Log.Logger.Warning("Could not fetch product with PartNo {PartNo} from Norce", partNo);
            return null;
        }

        var product = JsonSerializer.Deserialize<NorceApiProduct>(json);
        return product;
    }

    /// <summary>
    /// Retrieves a list of flags from the Norce API, optionally filtered by a specific culture code.
    /// </summary>
    /// <param name="cultureCode">The culture code used to filter the flags. Defaults to an empty string, indicating no filter.</param>
    /// <returns>
    /// Returns a <see cref="List{NorceApiFlag}"/> if flags are successfully retrieved and deserialized.
    /// If the flags cannot be fetched or an error occurs, returns <c>null</c>.
    /// </returns>
    public async Task<List<NorceApiFlag>?> ListFlags(string? cultureCode = "")
    {
        var json = await client.BaseGetAsync(Endpoints.Api.Product.ListFlags(cultureCode));
        if (string.IsNullOrWhiteSpace(json))
        {
            Log.Logger.Warning("Could not fetch flags from Norce");
        }

        var flags = JsonSerializer.Deserialize<List<NorceApiFlag>>(json);

        return flags;
    }
}
