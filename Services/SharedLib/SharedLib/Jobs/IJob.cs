namespace SharedLib.Jobs;

public interface IJob
{
    /// <summary>
    /// This is usually the class name of the job
    /// </summary>
    /// <returns></returns>
    string Id();

    /// <summary>
    /// Runs the job
    /// </summary>
    /// <returns></returns>
    Task Start(CancellationToken cancellationToken);

    /// <summary>
    /// Executes the job specific logic
    /// </summary>
    /// <returns></returns>
    Task Execute(CancellationToken cancellationToken);

}