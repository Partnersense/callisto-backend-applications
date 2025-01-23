using System.Text.Json;
using Elastic.CommonSchema;
using SharedLib.Models.Norce.Api;
using SharedLib.Serialization;
using Log = Serilog.Log;

namespace SharedLib.Clients.Norce;

public class NorceApiClient(INorceClient client)
{
    private readonly JsonSerializerOptions _jsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        Converters = { new NorceDateTimeConverter() }
    };


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

    /// <summary>
    /// Retrieves a list of products from the Norce API based on specified status codes, price list identifiers, and sales area.
    /// This method fetches product data that matches all the provided filtering criteria.
    /// </summary>
    /// <param name="statusSeed">List of status codes to filter products. Each status code represents a different product state in the system.</param>
    /// <param name="priceListSeed">List of price list identifiers to filter products. Products must be associated with at least one of these price lists.</param>
    /// <param name="SalesAreaId">The unique identifier of the sales area to retrieve products from.</param>
    /// <param name="traceId">Optional unique identifier for request tracing.</param>
    /// <returns>
    /// A list of <see cref="ListProducts2Response"/> objects containing the product information if the request is successful,
    /// or null if no products are found or if there's an error in the request.
    /// </returns>
    /// <exception cref="JsonException">Thrown when the response cannot be deserialized into the expected format.</exception>
    /// <exception cref="HttpRequestException">Thrown when there's an error in the HTTP request to the Norce API.</exception>
    /// <remarks>
    /// The method uses the BaseGetAsync method to make the API call and deserializes the JSON response into a strongly-typed list.
    /// If the API call returns an empty or whitespace response, the method logs a warning and returns null.
    /// </remarks>
    public async Task<List<ListProducts2Response>?> GetProducts(List<int> statusSeed, List<int> priceListSeed, int SalesAreaId)
    {
        var page = 1;
        var pageSize = 500;
        var products = new List<ListProducts2Response>();

        while (true)
        {
            var json = await client.BaseGetAsync(Endpoints.Api.Product.ListProducts2(statusSeed, priceListSeed, SalesAreaId, page, pageSize));
            if (string.IsNullOrWhiteSpace(json))
            {
                Log.Logger.Warning("Could not fetch products with statusSeed: {statusSeed}, PriceListSeed: {priceListSeed}, SalesAreaId: {salesAreaId}", string.Join(",", statusSeed), string.Join(",", priceListSeed, SalesAreaId));
                return null;
            }

            var serializedList = JsonSerializer.Deserialize<ApiItemWrapperResponse<ListProducts2Response>>(json, _jsonOptions);

            if (serializedList?.Items == null)
                return null;

            if(serializedList.Items.Count == 0)
                break;

            products.AddRange(serializedList.Items);
            page++;
        }

        return products;
    }
}
