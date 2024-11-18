using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Reflection;

namespace SharedLib.Middleware;

#pragma warning disable S3011
public class ConfigurationMonitorService
{
    private readonly ILogger<ConfigurationMonitorService> _logger;
    private readonly IServiceProvider _serviceProvider;

    public ConfigurationMonitorService(IServiceProvider serviceProvider, ILogger<ConfigurationMonitorService> logger)
    {
        _logger = logger;
        _serviceProvider = serviceProvider;

        // Dynamically subscribe to changes for all classes ending with "ModuleOptions"
        SubscribeToAllModuleOptionsChanges();
    }

    /// <summary>
    /// Subscribes to changes in all classes ending with "ModuleOptions" and logs a message when the options change for each class.
    /// </summary>
    private void SubscribeToAllModuleOptionsChanges()
    {
        var optionsTypes = Assembly.GetExecutingAssembly()
            .GetTypes()
            .Where(t => t.IsClass && !t.IsAbstract && t.Name.EndsWith("ModuleOptions"))
            .ToList();

        foreach (var optionsType in optionsTypes)
        {
            var subscribeMethod = typeof(ConfigurationMonitorService)
                .GetMethod(nameof(SubscribeToChanges), BindingFlags.Instance | BindingFlags.NonPublic)
                ?.MakeGenericMethod(optionsType);

            if (subscribeMethod is not null)
            {
                _logger.LogInformation("Registered options listener for {Method}", optionsType.Name);
                subscribeMethod.Invoke(this, new object[] { _serviceProvider });
            }
            else
            {
                _logger.LogWarning("Failed to register options listener for {Method}", optionsType.Name);
            }
        }
    }

    /// <summary>
    /// Subscribes to changes in the specified options and logs a message when the options change.
    /// </summary>
    /// <typeparam name="TOptions">The type of options to subscribe to.</typeparam>
    /// <param name="serviceProvider">The service provider used to resolve the options monitor.</param>
    /// <remarks>
    /// This method subscribes to changes in the specified options using an <see cref="IOptionsMonitor{TOptions}"/>.
    /// Whenever the options change, a log message is written to indicate that the configuration for the specified options has changed.
    /// </remarks>
    private void SubscribeToChanges<TOptions>(IServiceProvider serviceProvider) where TOptions : class, new()
    {
        var optionsMonitor = serviceProvider.GetRequiredService<IOptionsMonitor<TOptions>>();
        optionsMonitor.OnChange((options, name) =>
        {
            var prefix = typeof(TOptions).Name.Replace("ModuleOptions", "");
            _logger.LogInformation("Configuration for Options: {Prefix} changed", prefix);
        });
    }
}
