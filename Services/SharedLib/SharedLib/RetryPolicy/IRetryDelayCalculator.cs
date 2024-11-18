namespace SharedLib.RetryPolicy;

/// <summary>
/// Calculates delay using jitter policy to ensure that multiple clients don't all fall on the exact same timer,
/// thus continuing to flood the endpoint with requests at the same time.
/// </summary>
public interface IRetryDelayCalculator
{
    /// <summary>
    /// This method will calculate a sleep timer for a retry policy. This sleep timer
    /// is exponential to ensure that the endpoint has chance to recover before making a new attempt.
    /// The jitter calculated using Random() is there to ensure that multiple threads don't enter the exact same
    /// retry timers. Such a case would just keep flooding a too busy server with more requests without giving it a chance to recover
    /// in the event of a TooManyRequests error. The random calculation is locked, as Random() is not thread safe.
    /// The sleep timer would look something like this:
    /// Attempt 1: 1.01 - 1.2 seconds, Attempt 2: 2.01 - 2.2 seconds, Attempt 3: 4.01 - 4.2 seconds
    /// </summary>
    /// <param name="attemptNumber">Number of current attempt since start</param>
    /// <returns>timespan used for back off timer</returns>
    TimeSpan Calculate(int attemptNumber);
}