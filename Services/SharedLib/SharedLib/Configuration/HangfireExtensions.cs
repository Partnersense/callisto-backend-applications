using Hangfire;
using Hangfire.Console;
using Hangfire.Dashboard.BasicAuthorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using SharedLib.Options.Models;

namespace SharedLib.Configuration;
public static class HangfireExtensions
{
    public static void AddDefaultHangfireServer(this IServiceCollection services, string serverName)
    {
        services.AddHangfire(config =>
        {
            config.SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
                .UseSimpleAssemblyNameTypeSerializer()
                .UseDefaultTypeSerializer()
                .UseInMemoryStorage()
                .UseConsole()
                .UseSerilogLogProvider()
                .UseColouredConsoleLogProvider();
        });

        services.AddHangfireServer(config => { config.ServerName = serverName; });
    }

    public static void AddDefaultHangfireDashboard(this WebApplication app, HangfireModuleOptions options)
    {
        app.UseHangfireDashboard(options!.Endpoint, new DashboardOptions
        {
            Authorization = new[] { new BasicAuthAuthorizationFilter(new BasicAuthAuthorizationFilterOptions
            {
                RequireSsl = false,
                SslRedirect = false,
                LoginCaseSensitive = true,
                Users = new []
                {
                    new BasicAuthAuthorizationUser
                    {
                        Login = options!.DashboardUsername,
                        PasswordClear =  options.DashboardPassword
                    }
                }

            }) }
        });

        app.MapHangfireDashboard();
    }
}
