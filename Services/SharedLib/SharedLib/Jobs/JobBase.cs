using Microsoft.Extensions.Logging;


namespace SharedLib.Jobs;
public abstract class JobBase<T>(ILogger<T> logger) : IJob
{
    protected readonly ILogger<T> Logger = logger ?? throw new ArgumentNullException(nameof(logger));

    public string Id()
    {
        return typeof(T).Name;
    }

    public async Task Start(CancellationToken cancellationToken)
    {
        Logger.LogInformation("Starting Job: {Id}", Id());

        await Execute(cancellationToken);
    }

    public abstract Task Execute(CancellationToken cancellationToken);
}
