using SharedLib.Models.Sage;

namespace SharedLib.Clients.Sage;
public interface ISageClient
{
    /// <summary>
    /// Fetch products from a specific warehouse.
    /// </summary>
    /// <param name="warehouseCode"></param>
    /// <returns></returns>
    Task<QueryResult?> FetchProductsDetails(string warehouseCode);

    /// <summary>
    /// Fetch product prices from a specific warehouse.
    /// </summary>
    /// <param name="paramKeyValues"></param>
    /// <returns></returns>
    Task<QueryResult?> FetchPrices(Dictionary<string, string>? paramKeyValues = null);

    /// <summary>
    /// Fetch product stock from a specific warehouse.
    /// </summary>
    /// <param name="warehouseCode"></param>
    /// <returns></returns>
    Task<List<SageStockResultLine>> FetchProductStock(string warehouseCode);
    Task<T?> GetEntity<T>(string soapAction, string soapBody);
}
