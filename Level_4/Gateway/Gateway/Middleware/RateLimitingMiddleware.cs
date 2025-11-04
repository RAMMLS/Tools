using System.Collections.Concurrent;
using Gateway.Models;

namespace Gateway.Middleware
{
    public class RateLimitingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ConcurrentDictionary<string, ClientRateLimit> _clientLimits;
        
        public RateLimitingMiddleware(RequestDelegate next)
        {
            _next = next;
            _clientLimits = new ConcurrentDictionary<string, ClientRateLimit>();
        }

        public async Task InvokeAsync(HttpContext context, RouteManager routeManager)
        {
            var clientIp = GetClientIp(context);
            var route = routeManager.GetRouteForPath(context.Request.Path, context.Request.Method);
            
            if (route != null && route.RateLimit > 0)
            {
                var clientLimit = _clientLimits.GetOrAdd(clientIp, 
                    ip => new ClientRateLimit { Limit = route.RateLimit });

                if (DateTime.Now - clientLimit.WindowStart > TimeSpan.FromMinutes(1))
                {
                    clientLimit.RequestCount = 0;
                    clientLimit.WindowStart = DateTime.Now;
                }

                if (clientLimit.RequestCount >= clientLimit.Limit)
                {
                    context.Response.StatusCode = 429; // Too Many Requests
                    await context.Response.WriteAsync("Rate limit exceeded");
                    return;
                }

                clientLimit.RequestCount++;
            }

            await _next(context);
        }

        private string GetClientIp(HttpContext context)
        {
            return context.Connection.RemoteIpAddress?.ToString() ?? "unknown";
        }
    }

    public class ClientRateLimit
    {
        public int RequestCount { get; set; }
        public int Limit { get; set; }
        public DateTime WindowStart { get; set; } = DateTime.Now;
    }
}
