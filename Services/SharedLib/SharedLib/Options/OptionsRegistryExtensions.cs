using Microsoft.Extensions.DependencyInjection;
using SharedLib.Options.Models;

namespace SharedLib.Options;

/// <summary>
/// Manual registration of options in the project
/// This is the only instance in the code where you need to handle
/// options manually, explicitly registering them.
/// </summary>
public static class OptionsRegistryExtensions
{
    /// <summary>
    /// Registers and binds all options in the project.
    /// Options that you want to poll for automatically
    /// are present under in appsettings.json
    /// </summary>
    /// <param name="services"></param>
    /// <returns>The updated WebApplicationBuilder instance.</returns>
    public static void UseHangfireOptionsRegistry(this IServiceCollection services)
    {
        services.AddOptions<HangfireModuleOptions>()
            .BindConfiguration("Options:HangfireConfiguration")
            .ValidateDataAnnotations()
            .ValidateOnStart();

        services.AddOptions<List<JobModuleOptions>>()
            .BindConfiguration("Options:JobConfigurations")
            .ValidateDataAnnotations()
            .ValidateOnStart();

    }

    public static void UseNorceOptionsRegistry(this IServiceCollection services)
    {
        services.AddOptions<NorceBaseModuleOptions>()
            .BindConfiguration("Options:NorceBaseConfiguration")
            .ValidateDataAnnotations()
            .ValidateOnStart();
    }
    
    public static void UseSageClientOptionsRegistry(this IServiceCollection services)
    {
        services.AddOptions<SageModuleOptions>()
            .BindConfiguration("Options:SageClientConfiguration")
            .ValidateDataAnnotations()
            .ValidateOnStart();
    }

    public static void UseStorageOptionsRegistry(this IServiceCollection services)
    {
        services.AddOptions<StorageModuleOptions>()
            .BindConfiguration("Options:StorageConfiguration")
            .ValidateDataAnnotations()
            .ValidateOnStart();
    }
}