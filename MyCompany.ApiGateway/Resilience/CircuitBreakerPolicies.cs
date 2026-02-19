using Polly;
using Polly.Extensions.Http;
using System;

namespace MyCompany.ApiGateway.Resilience
{
    public static class CircuitBreakerPolicies
    {
        public static IAsyncPolicy<HttpResponseMessage> GetCircuitBreakerPolicy()
        {
            return HttpPolicyExtensions
                .HandleTransientHttpError()
                .CircuitBreakerAsync(5, TimeSpan.FromSeconds(30));
        }
    }
}
