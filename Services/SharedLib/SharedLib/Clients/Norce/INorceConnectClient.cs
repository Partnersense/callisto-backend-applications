using Enferno.Services.StormConnect.Contracts.Product;
using Enferno.Services.StormConnect.Contracts.Product.Models;

namespace SharedLib.Clients.Norce;

public interface INorceConnectClient
{
    Task<HttpResponseMessage> PostAsync<THeader>(string serviceEndpoint, string json, THeader header, string? applicationId = null);
    Task<HttpResponseMessage> PostAsync(string serviceEndpoint, string json, string? applicationId = null);
    Task<string> GetAsync(string serviceEndpoint, string? applicationId = null);
    Task<string> Ping(string? applicationId = null);
    Task<bool> ImportProducts(IEnumerable<Product> products, ProductHeader header, int batchSize, string? applicationId = null);
    Task<bool> ImportOnHands(IEnumerable<SkuOnhand> onHands, int batchSize, string? applicationId = null);
    Task<bool> ImportSkuPriceLists(IEnumerable<SkuPriceList> prices, int batchSize, string? applicationId = null);
}