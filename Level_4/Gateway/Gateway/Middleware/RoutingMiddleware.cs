using Gateway.Models;
using Gateway.Services;

namespace Gateway.Middleware
{
    public class RoutingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly HttpClient _httpClient;

        public RoutingMiddleware(RequestDelegate next)
        {
            _next = next;
            _httpClient = new HttpClient();
        }

        public async Task InvokeAsync(HttpContext context, RouteManager routeManager, ServiceDiscovery serviceDiscovery)
        {
            var route = routeManager.GetRouteForPath(context.Request.Path, context.Request.Method);
            
            if (route == null)
            {
                context.Response.StatusCode = 404;
                await context.Response.WriteAsync("Route not found");
                return;
            }

            var loadBalancer = LoadBalancerFactory.Create(route.LoadBalancingStrategy);
            var endpoint = loadBalancer.GetNextEndpoint(route.Endpoints);

            if (endpoint == null)
            {
                context.Response.StatusCode = 503;
                await context.Response.WriteAsync("No healthy endpoints available");
                return;
            }

            await ForwardRequest(context, endpoint, route);
        }

        private async Task ForwardRequest(HttpContext context, ServiceEndpoint endpoint, RouteConfig route)
        {
            try
            {
                var targetUrl = $"{endpoint.Url}{context.Request.Path}{context.Request.QueryString}";
                var requestMessage = CreateHttpRequestMessage(context, targetUrl);

                using var response = await _httpClient.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead, context.RequestAborted);
                
                // Копируем статус код и заголовки
                context.Response.StatusCode = (int)response.StatusCode;
                foreach (var header in response.Headers)
                {
                    context.Response.Headers[header.Key] = header.Value.ToArray();
                }

                // Копируем тело ответа
                await response.Content.CopyToAsync(context.Response.Body);
            }
            catch (TaskCanceledException)
            {
                context.Response.StatusCode = 504; // Gateway Timeout
                await context.Response.WriteAsync("Request timeout");
            }
            catch (Exception ex)
            {
                context.Response.StatusCode = 500;
                await context.Response.WriteAsync($"Gateway error: {ex.Message}");
            }
        }

        private HttpRequestMessage CreateHttpRequestMessage(HttpContext context, string targetUrl)
        {
            var requestMessage = new HttpRequestMessage
            {
                RequestUri = new Uri(targetUrl),
                Method = new HttpMethod(context.Request.Method)
            };

            // Копируем заголовки
            foreach (var header in context.Request.Headers)
            {
                if (!header.Key.Equals("Host", StringComparison.OrdinalIgnoreCase))
                {
                    requestMessage.Headers.TryAddWithoutValidation(header.Key, header.Value.ToArray());
                }
            }

            // Копируем тело запроса
            if (context.Request.ContentLength > 0)
            {
                requestMessage.Content = new StreamContent(context.Request.Body);
                if (context.Request.Headers.ContainsKey("Content-Type"))
                {
                    requestMessage.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(
                        context.Request.Headers["Content-Type"].ToString());
                }
            }

            return requestMessage;
        }
    }
}
