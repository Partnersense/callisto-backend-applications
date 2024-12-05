
namespace SharedLib.Clients.Norce
{
    public interface INorceQueryClient
    {
        Task<string> GetApplicationMetadata();
        Task<string> GetAsync(string query, string? applicationId = null);
    }
}