namespace SharedLib.Clients.Norce;

public class NorceQueryClient(INorceClient client) : INorceQueryClient
{
    public async Task<string> GetAsync(string query, string? applicationId = null)
    {
        return await client.BaseGetAsync(Endpoints.GetEndpoint(Endpoints.Query.Base, query), applicationId);
    }

    public async Task<string> GetApplicationMetadata()
    {
        return await client.BaseGetAsync(Endpoints.Query.Application.MetaData);
    }
}