using Gateway.Models;

namespace Gateway.Middleware
{
    public class AuthenticationMiddleware
    {
        private readonly RequestDelegate _next;

        public AuthenticationMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context, RouteManager routeManager)
        {
            var route = routeManager.GetRouteForPath(context.Request.Path, context.Request.Method);
            
            if (route?.RequiresAuthentication == true)
            {
                if (!IsAuthenticated(context))
                {
                    context.Response.StatusCode = 401;
                    await context.Response.WriteAsync("Unauthorized");
                    return;
                }
            }

            await _next(context);
        }

        private bool IsAuthenticated(HttpContext context)
        {
            // Простая проверка API ключа в заголовке
            if (context.Request.Headers.TryGetValue("X-API-Key", out var apiKey))
            {
                return apiKey == "secret-key-123"; // В реальном приложении проверять в базе данных
            }

            // Проверка JWT токена
            var authHeader = context.Request.Headers["Authorization"].FirstOrDefault();
            if (!string.IsNullOrEmpty(authHeader) && authHeader.StartsWith("Bearer "))
            {
                var token = authHeader.Substring(7);
                return ValidateJwtToken(token);
            }

            return false;
        }

        private bool ValidateJwtToken(string token)
        {
            // Упрощенная проверка JWT
            // В реальном приложении использовать библиотеку для валидации JWT
            return !string.IsNullOrEmpty(token) && token.Length > 10;
        }
    }
}
