namespace SharedLib.RetryPolicy;

public class ExponentialBackoffWithJitterCalculator : IRetryDelayCalculator
{
    private readonly Random _random = new();
    private readonly object _randomLock = new();

    public TimeSpan Calculate(int attemptNumber)
    {
        int jitter;
        lock (_randomLock)
        {
            jitter = _random.Next(10, 200);
        }
        return TimeSpan.FromSeconds(Math.Pow(2, attemptNumber - 1)) + TimeSpan.FromMilliseconds(jitter);
    }
}
