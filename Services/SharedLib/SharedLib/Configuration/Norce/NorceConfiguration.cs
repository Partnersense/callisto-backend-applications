using Microsoft.Extensions.DependencyInjection;
using SharedLib.Clients.Norce;
using SharedLib.RetryPolicy;

namespace SharedLib.Configuration.Norce
{
    public static class NorceConfiguration
    {
        /// <summary>
        /// + Retrypolicies
        /// + Query client
        /// + Api client
        /// + Connect client
        /// + Token client
        /// + Polly stuff
        /// </summary>
        /// <param name="services"></param>
        /// <exception cref="InvalidOperationException"></exception>
        public static void ConfigureDefaultNorceClients(this IServiceCollection services)
        {
            services.AddSingleton<INorceRetryPolicy, NorceRetryPolicy>();
            services.AddSingleton<IRetryDelayCalculator, ExponentialBackoffWithJitterCalculator>();
            services.AddHttpClient<INorceClient, NorceClient>(nameof(NorceClient));
            services.AddTransient<INorceClient, NorceClient>();
        }
    }
}