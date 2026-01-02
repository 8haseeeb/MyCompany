using Polly;
using System;

namespace MyCompany.ApiGateway.Resilience
{
    public static class RetryPolicies
    {
        public static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy()
        {
            return Policy.HandleResult<HttpResponseMessage>(r => !r.IsSuccessStatusCode)
                         .RetryAsync(3);
        }
    }
}
