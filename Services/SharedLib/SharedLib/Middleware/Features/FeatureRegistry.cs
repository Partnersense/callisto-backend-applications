using Microsoft.Extensions.Logging;
using Microsoft.FeatureManagement;

namespace SharedLib.Middleware.Features;
/// <summary>
/// Provides nice bootstrap logging for features.
/// </summary>
public class FeatureRegistry : IFeatureRegistry
{
    private readonly IFeatureManager _featureManager;
    private readonly ILogger<FeatureRegistry> _logger;

    public FeatureRegistry(IFeatureManager featureManager, ILogger<FeatureRegistry> logger)
    {
        _featureManager = featureManager;
        _logger = logger;
    }

    /// <summary>
    /// Asynchronously retrieves the list of enabled features.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation.
    /// The task result contains the list of enabled features.</returns>
    public async Task ListEnabledFeaturesAsync()
    {
        var featureNames = new List<string>();
        await foreach (var feature in _featureManager.GetFeatureNamesAsync())
        {
            featureNames.Add(feature);
        }

        var tasks = featureNames.Select(async feature =>
        {
            bool isEnabled = await _featureManager.IsEnabledAsync(feature);
            return (Feature: feature, IsEnabled: isEnabled);
        });

        var results = await Task.WhenAll(tasks);

        foreach (var result in results)
        {
            var statusIcon = result.IsEnabled ? "💚" : "🩶";
            var isEnabled = result.IsEnabled ? "enabled" : "disabled";
            _logger.LogInformation("Registered feature {Name} is {Enabled} {Icon}", result.Feature, isEnabled, statusIcon);
        }
    }
}

public interface IFeatureRegistry
{
    public Task ListEnabledFeaturesAsync();
}