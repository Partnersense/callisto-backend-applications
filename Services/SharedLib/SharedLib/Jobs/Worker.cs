using Hangfire;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using SharedLib.Options.Models;

namespace SharedLib.Jobs;

public class Worker(IServiceProvider services, IOptions<List<JobModuleOptions>> jobConfigurations, IHostEnvironment environment)
    : BackgroundService
{
    private readonly List<JobModuleOptions> _jobConfigurations = jobConfigurations.Value;

    /// <summary>
    /// Executes all registered services of type <see cref="IJob"/>.
    /// The ID of the bound <see cref="JobModuleOptions"/> MUST be the same as the job ID specified in the service.
    /// </summary>
    /// <param name="stoppingToken"></param>
    /// <returns></returns>
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        foreach (var job in services.GetServices<IJob>())
        {
            if (environment.IsDevelopment())
            {
                await job.Start(stoppingToken);
            }
            else
            {
                var config = _jobConfigurations.Find(config => config.JobId == job.Id());
                RecurringJob.AddOrUpdate(config?.JobId, () => job.Start(stoppingToken), config?.CronExpression);
            }
        }

        while (!stoppingToken.IsCancellationRequested)
        {
            await Task.Delay(1000, stoppingToken);
        }
    }
}