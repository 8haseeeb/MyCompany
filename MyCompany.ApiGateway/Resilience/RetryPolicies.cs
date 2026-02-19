using Polly;
using Polly.Extensions.Http;
using System;

namespace MyCompany.ApiGateway.Resilience
{
    public static class RetryPolicies
    {
        public static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy()
        {
            return HttpPolicyExtensions
                .HandleTransientHttpError()
                .RetryAsync(3);
        }
    }
}
