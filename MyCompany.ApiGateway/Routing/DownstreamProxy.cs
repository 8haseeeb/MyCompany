using Serilog;
using System.Net.Http.Headers;

namespace MyCompany.ApiGateway.Routing
{
    public class DownstreamProxy
    {
        private readonly HttpClient _httpClient;

        public DownstreamProxy(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task ProxyAsync(HttpContext context, string targetUrl)
        {
            // SERILOG - outgoing request
            Log.Information(
                "Forwarding request {Method} {Path} to {TargetUrl}",
                context.Request.Method,
                context.Request.Path,
                targetUrl
            );

            var requestMessage = new HttpRequestMessage
            {
                Method = new HttpMethod(context.Request.Method),
                RequestUri = new Uri(targetUrl + context.Request.QueryString)
            };

            var isPromotionsBasicHealth = string.Equals(
                context.Request.Path.Value,
                "/api/v1/health",
                StringComparison.OrdinalIgnoreCase);

            //  COPY HEADERS 
            foreach (var header in context.Request.Headers)
            {
                if (header.Key.Equals("Host", StringComparison.OrdinalIgnoreCase))
                    continue;

                if (!requestMessage.Headers.TryAddWithoutValidation(header.Key, header.Value.ToArray()))
                {
                    requestMessage.Content ??= new StreamContent(Stream.Null);
                    requestMessage.Content.Headers.TryAddWithoutValidation(header.Key, header.Value.ToArray());
                }
            }

            // Expired/invalid browser tokens must not block DB health (JWT would return 401 otherwise).
            if (isPromotionsBasicHealth)
                requestMessage.Headers.Remove("Authorization");

            //  COPY BODY (POST / PUT / PATCH) 
            if (!HttpMethods.IsGet(context.Request.Method) &&
                !HttpMethods.IsHead(context.Request.Method) &&
                !HttpMethods.IsDelete(context.Request.Method) &&
                context.Request.ContentLength > 0)
            {
                context.Request.EnableBuffering();
                context.Request.Body.Position = 0;

                var memoryStream = new MemoryStream();
                await context.Request.Body.CopyToAsync(memoryStream);
                memoryStream.Position = 0;

                var streamContent = new StreamContent(memoryStream);
                streamContent.Headers.ContentType = MediaTypeHeaderValue.Parse(context.Request.ContentType ?? "application/json");
                requestMessage.Content = streamContent;
            }

            //  SEND REQUEST 
            try
            {
                var response = await _httpClient.SendAsync(
                    requestMessage,
                    HttpCompletionOption.ResponseHeadersRead,
                    context.RequestAborted);

                //  SERILOG - response info
                Log.Information(
                    "Received response {StatusCode} from {TargetUrl}. Headers: {Headers}",
                    (int)response.StatusCode,
                    targetUrl,
                    string.Join(", ", response.Headers.Select(h => $"{h.Key}={string.Join("|", h.Value)}"))
                );

                //  COPY RESPONSE (single copy; no X-Echo duplication to avoid duplicate/confusing headers)
                context.Response.StatusCode = (int)response.StatusCode;

                foreach (var header in response.Headers)
                {
                    if (!header.Key.Equals("Transfer-Encoding", StringComparison.OrdinalIgnoreCase))
                        context.Response.Headers[header.Key] = header.Value.ToArray();
                }

                foreach (var header in response.Content.Headers)
                {
                    if (!header.Key.Equals("Transfer-Encoding", StringComparison.OrdinalIgnoreCase))
                        context.Response.Headers[header.Key] = header.Value.ToArray();
                }

                await response.Content.CopyToAsync(context.Response.Body);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "❌ PROXY ERROR: Could not reach {TargetUrl}. Message: {Message}", targetUrl, ex.Message);

                if (context.Response.HasStarted)
                    throw;

                if (ex is OperationCanceledException)
                {
                    Log.Error("⚠️ PROXY TIMEOUT: The request to {TargetUrl} was canceled or timed out.", targetUrl);
                    context.Response.StatusCode = StatusCodes.Status504GatewayTimeout;
                    context.Response.ContentType = "application/json";
                    await context.Response.WriteAsync(
                        System.Text.Json.JsonSerializer.Serialize(new { message = "Downstream request timed out.", detail = ex.Message }));
                    return;
                }

                var isCircuitBroken = ex.GetType().Name.Contains("BrokenCircuit", StringComparison.Ordinal)
                    || (ex.InnerException?.GetType().Name.Contains("BrokenCircuit", StringComparison.Ordinal) ?? false);

                context.Response.StatusCode = isCircuitBroken
                    ? StatusCodes.Status503ServiceUnavailable
                    : StatusCodes.Status502BadGateway;
                context.Response.ContentType = "application/json";
                var msg = isCircuitBroken
                    ? "Downstream service temporarily unavailable (circuit open). Try again shortly."
                    : "API gateway could not reach the downstream service. Check SSO_URL / PROMOTIONS_URL and that containers are running.";
                await context.Response.WriteAsync(
                    System.Text.Json.JsonSerializer.Serialize(new { message = msg, detail = ex.Message }));
                return;
            }

        }
    }
}
