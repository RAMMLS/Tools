using Gateway.Models;

namespace Gateway.Middleware
{
    public class LoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<LoggingMiddleware> _logger;

        public LoggingMiddleware(RequestDelegate next, ILogger<LoggingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var startTime = DateTime.Now;
            var request = CreateGatewayRequest(context);

            _logger.LogInformation($"Request started: {request}");

            try
            {
                await _next(context);
                
                var duration = DateTime.Now - startTime;
                _logger.LogInformation($"Request completed: {request} - Status: {context.Response.StatusCode} - Duration: {duration.TotalMilliseconds}ms");
            }
            catch (Exception ex)
            {
                var duration = DateTime.Now - startTime;
                _logger.LogError(ex, $"Request failed: {request} - Duration: {duration.TotalMilliseconds}ms");
                throw;
            }
        }

        private GatewayRequest CreateGatewayRequest(HttpContext context)
        {
            return new GatewayRequest
            {
                ClientIp = context.Connection.RemoteIpAddress?.ToString() ?? "unknown",
                Method = context.Request.Method,
                Path = context.Request.Path,
                Headers = context.Request.Headers.ToDictionary(h => h.Key, h => h.Value.ToString()),
                UserAgent = context.Request.Headers["User-Agent"].ToString()
            };
        }
    }
}
