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
                
                // If it's a timeout, log it specifically
                if (ex is TaskCanceledException || ex is OperationCanceledException)
                {
                    Log.Error("⚠️ PROXY TIMEOUT: The request to {TargetUrl} took too long and was canceled.", targetUrl);
                }

                throw;
            }

        }
    }
}
