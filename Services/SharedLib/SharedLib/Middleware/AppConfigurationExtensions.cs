using Azure.Identity;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.AzureAppConfiguration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

#pragma warning disable S2583

namespace SharedLib.Middleware;

/// <summary>
/// Provides extension methods to configure application settings.
/// </summary>
public static class AppConfigurationExtensions
{
    /// <summary>
    /// Configures the application to use custom app configuration settings.
    /// This method decides whether to use Azure App Configuration based on environment variables
    /// and the current environment (development or other).
    /// </summary>
    /// <param name="builder">The WebApplicationBuilder to configure.</param>
    /// <param name="serviceConfigPrefix">The prefix for the optional service specific configuration</param>
    /// <returns>The configured WebApplicationBuilder.</returns>
    /// <remarks>
    /// In development environment, it checks for 'USE_AZURE_APP_CONFIGURATION', 'USE_REMOTE_ENVIRONMENT_TYPE',
    /// and 'AZURE_APP_CONFIGURATION_CONNECTION' environment variables to determine the configuration source.
    /// For non-development environments, it defaults to using Azure App Configuration with the environment name.
    /// Throws InvalidOperationException if Azure App Configuration Connection string is not provided in non-development environments.
    /// </remarks>
    public static WebApplicationBuilder UseCustomAppConfiguration(this WebApplicationBuilder builder, string? serviceConfigPrefix = "")
    {
        var env = builder.Environment;

        // Retrieve configuration settings from environment variables.
        var azureAppConfigurationConnection = Environment.GetEnvironmentVariable("AZURE_APP_CONFIGURATION_CONNECTION");
        var serviceAppConfigurationConnection = Environment.GetEnvironmentVariable($"{serviceConfigPrefix}_SERVICE_AZURE_APP_CONFIGURATION_CONNECTION");
        var useAzureAppConfiguration = Environment.GetEnvironmentVariable("USE_AZURE_APP_CONFIGURATION");
        var useRemoteEnvironmentType = Environment.GetEnvironmentVariable("USE_REMOTE_ENVIRONMENT_TYPE");
        var useLocalConfiguration = Environment.GetEnvironmentVariable("USE_LOCAL_APP_CONFIGURATION");

        // Configuration for development environment.
        if (env.IsDevelopment())
        {
            // Use Azure App Configurations if specified by environment variables.
            if (bool.TryParse(useAzureAppConfiguration, out var useAzure) && useAzure)
            {
                // Adds shared configuration
                builder.ValidateAndUseConfiguration(azureAppConfigurationConnection, useRemoteEnvironmentType);
                // Adds service specific configuration
                builder.ValidateAndUseConfiguration(serviceAppConfigurationConnection, useRemoteEnvironmentType);
            }

            // Use local App Configuration if specified by environment variables, or not using azure app config
            builder.ValidateAndUseConfiguration(null, null, useLocalConfiguration);
        }
        else
        {
            if (string.IsNullOrEmpty(azureAppConfigurationConnection) &&
                string.IsNullOrEmpty(serviceAppConfigurationConnection))
            {
                throw new InvalidOperationException("Azure App Configuration Connection string is not provided.");
            }

            // Configuration for non-development environments defaults to using Azure App Configuration.
            builder.ValidateAndUseConfiguration(azureAppConfigurationConnection, env.EnvironmentName);
            builder.ValidateAndUseConfiguration(serviceAppConfigurationConnection, env.EnvironmentName);
        }

        return builder;
    }

    /// <summary>
    /// Configures the application to use Azure App Configuration with custom settings.
    /// It selects configuration settings based on the environment name and sets up refresh with a Sentinel key.
    /// </summary>
    /// <param name="builder">The WebApplicationBuilder to configure.</param>
    /// <param name="connection">The connection string to Azure App Configuration.</param>
    /// <param name="environmentName">The current .NET environment name.</param>
    /// <returns>The configured WebApplicationBuilder.</returns>
    /// <remarks>
    /// Logs the use of Azure Configuration for the specified environment.
    /// Registers a Sentinel key for dynamic refresh, with a cache expiration time of 30 seconds.
    /// </remarks>
    private static void UseAzureAppConfiguration(this WebApplicationBuilder builder, string connection,
        string environmentName)
    {
        // Get Refresh Interval from Environment variables
        var azureAppConfigurationRefreshInMinutes = int.TryParse(Environment.GetEnvironmentVariable("AZURE_APP_CONFIGURATION_REFRESH_MINUTES"), out int refreshInMinutes)
            ? refreshInMinutes
            : 15;

        IConfigurationRefresher? refresher = null;

        builder.Configuration.AddAzureAppConfiguration(options =>
        {
            options.Connect(connection)
                .Select("*", environmentName)
                .UseFeatureFlags(featureFlagOptions =>
                {
                    featureFlagOptions.CacheExpirationInterval = TimeSpan.FromMinutes(azureAppConfigurationRefreshInMinutes);
                })
                .ConfigureKeyVault(kv =>
                {
                    kv.SetCredential(new DefaultAzureCredential());
                })
                .ConfigureRefresh(refreshOptions =>
                    refreshOptions.Register("Sentinel", environmentName, true)
                        .SetCacheExpiration(TimeSpan.FromMinutes(azureAppConfigurationRefreshInMinutes)));
            Console.WriteLine("Debug: Successfully connected to Azure App Configuration");

        refresher = options.GetRefresher();
        });

        builder.Services.AddAzureAppConfiguration();

        if (refresher != null) 
            builder.Services.AddSingleton(refresher);

        Console.WriteLine($"⚡️Using Azure App Configuration for {environmentName}, will refresh every {azureAppConfigurationRefreshInMinutes} minutes");
    }



    /// <summary>
    /// Configures the application to use appsettings.json for configuration settings.
    /// It includes base appsettings.json and environment-specific files.
    /// </summary>
    /// <param name="builder">The WebApplicationBuilder to configure.</param>
    /// <returns>The configured WebApplicationBuilder.</returns>
    /// <remarks>
    /// Logs the use of AppSettings Configuration for the current environment.
    /// Uses optional environment-specific appsettings (e.g., appsettings.Production.json) if available.
    /// </remarks>
    private static void UseAppSettingsConfiguration(this WebApplicationBuilder builder)
    {
        var env = builder.Environment;
        Console.WriteLine($"⚒️Using Appsettings Configuration for {env.EnvironmentName}");
        builder.Configuration.SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", true, true)
            .AddJsonFile($"appsettings.{env.EnvironmentName}.json", true, true);

        builder.Services.AddSingleton<IConfigurationRefresher, DefaultConfigurationRefresher>();
    }

    private static void ValidateAndUseConfiguration(this WebApplicationBuilder builder, string? appConfigurationConnection = "", string? useRemoteEnvironmentType = "", string? useLocalConfiguration = "")
    {
        if (!string.IsNullOrEmpty(useRemoteEnvironmentType) &&
            !string.IsNullOrEmpty(appConfigurationConnection))
        {
            builder.UseAzureAppConfiguration(appConfigurationConnection, useRemoteEnvironmentType);
        }

        // Use local App Configuration if specified by environment variables, or not using azure app config
        if (bool.TryParse(useLocalConfiguration, out var useLocalConfig) && useLocalConfig)
        {
            builder.UseAppSettingsConfiguration();
        }
    }
}
