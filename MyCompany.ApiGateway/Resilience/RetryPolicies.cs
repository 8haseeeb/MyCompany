using Polly;
using Polly.Extensions.Http;
using System;

namespace MyCompany.ApiGateway.Resilience
{
    public static class RetryPolicies
    {
        /// <summary>
        /// Retry with exponential backoff (1s, 2s, 4s) + jitter to avoid thundering herd on failing downstream.
        /// </summary>
        public static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy()
        {
            return HttpPolicyExtensions
                .HandleTransientHttpError()
                .WaitAndRetryAsync(
                    retryCount: 3,
                    sleepDurationProvider: (attempt) => TimeSpan.FromSeconds(Math.Pow(2, attempt)) + TimeSpan.FromMilliseconds(Random.Shared.Next(0, 500)),
                    onRetry: (_, timeSpan, attempt, _) => { /* optional: log retry */ });
        }
    }
}
