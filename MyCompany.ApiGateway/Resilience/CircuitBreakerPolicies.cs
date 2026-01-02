using Polly;
using System;

namespace MyCompany.ApiGateway.Resilience
{
    public static class CircuitBreakerPolicies
    {
        public static IAsyncPolicy<HttpResponseMessage> GetCircuitBreakerPolicy()
        {
            return Policy.HandleResult<HttpResponseMessage>(r => !r.IsSuccessStatusCode)
                         .CircuitBreakerAsync(2, TimeSpan.FromSeconds(30));
        }
    }
}
