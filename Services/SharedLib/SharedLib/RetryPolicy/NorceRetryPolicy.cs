using System.Net;
using Microsoft.Extensions.Logging;
using Polly;
using SharedLib.Helpers;

namespace SharedLib.RetryPolicy;

public class NorceRetryPolicy : INorceRetryPolicy
{
    private readonly IRetryDelayCalculator _delayCalculator;
    private readonly ILogger<NorceRetryPolicy> _logger;
    private const int MaxRetries = 3;
    public IAsyncPolicy<HttpResponseMessage> BasicPolicy { get; }

    public NorceRetryPolicy(IRetryDelayCalculator delayCalculator, ILogger<NorceRetryPolicy> logger)
    {
        _delayCalculator = delayCalculator;
        _logger = logger;
        BasicPolicy = Policy.HandleResult<HttpResponseMessage>(ex => !ex.IsSuccessStatusCode)
            .WaitAndRetryAsync(1, _delayCalculator.Calculate,
                onRetry: (exception, sleepDuration, attemptNumber, context) =>
                {
                    _logger.LogInformation("A {Reason} error occurred. Trying again in {SleepDuration}. Attempt: {AttemptNumber}/{MaxRetries}", exception.Result.ReasonPhrase, sleepDuration, attemptNumber, MaxRetries);
                    throw new HttpException(exception.Result.StatusCode,
                        exception.Result.Content.ReadAsStringAsync().Result);
                });
    }

    public IAsyncPolicy<HttpResponseMessage> InitiateNorceHttpRetryPolicy(Action tokenRefresh)
    {
        return Policy.HandleResult<HttpResponseMessage>(ex =>
            {
                switch (ex.StatusCode)
                {
                    case HttpStatusCode.Unauthorized:
                    case HttpStatusCode.TooManyRequests:
                    case HttpStatusCode.RequestTimeout:
                        return true;
                    default:
                        return false;
                }
            })
            .WaitAndRetryAsync(MaxRetries, _delayCalculator.Calculate, onRetry: (exception, sleepDuration, attemptNumber, context) =>
            {
                _logger.LogInformation("A {Reason} error occurred. Trying again in {SleepDuration}. Attempt: {AttemptNumber}/{MaxRetries}", exception.Result.ReasonPhrase, sleepDuration, attemptNumber, MaxRetries);
                if (exception.Result is HttpResponseMessage { StatusCode: HttpStatusCode.Unauthorized })
                {
                    tokenRefresh();
                }
            });
    }

}
