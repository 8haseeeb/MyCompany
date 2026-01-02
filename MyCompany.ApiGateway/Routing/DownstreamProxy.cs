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
                !HttpMethods.IsDelete(context.Request.Method))
            {
                context.Request.EnableBuffering();

                var streamContent = new StreamContent(context.Request.Body);
                streamContent.Headers.ContentType =
                    MediaTypeHeaderValue.Parse(context.Request.ContentType ?? "application/json");

                requestMessage.Content = streamContent;

                context.Request.Body.Position = 0;
            }

            //  SEND REQUEST 
            var response = await _httpClient.SendAsync(
                requestMessage,
                HttpCompletionOption.ResponseHeadersRead,
                context.RequestAborted);

            //  SERILOG - response info
            Log.Information(
                "Received response {StatusCode} from {TargetUrl}",
                (int)response.StatusCode,
                targetUrl
            );

            //  COPY RESPONSE 
            context.Response.StatusCode = (int)response.StatusCode;

            foreach (var header in response.Headers)
                context.Response.Headers[header.Key] = header.Value.ToArray();

            foreach (var header in response.Content.Headers)
                context.Response.Headers[header.Key] = header.Value.ToArray();

            context.Response.Headers.Remove("transfer-encoding");

            await response.Content.CopyToAsync(context.Response.Body);
        }
    }
}
