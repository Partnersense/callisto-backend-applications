using Microsoft.Extensions.Configuration.AzureAppConfiguration;

namespace SharedLib.Middleware;

public class DefaultConfigurationRefresher() : IConfigurationRefresher
{
    public Task RefreshAsync(CancellationToken cancellationToken = new CancellationToken())
    {
        return Task.CompletedTask;
    }

    public Task<bool> TryRefreshAsync(CancellationToken cancellationToken = new CancellationToken())
    {
        return Task.FromResult(true);
    }

    public void ProcessPushNotification(PushNotification pushNotification, TimeSpan? maxDelay = null)
    {
        return;
    }

    public Uri AppConfigurationEndpoint { get; } = new Uri("https://localhost:7066");
}