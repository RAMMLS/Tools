using Gateway.Middleware;
using Gateway.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Gateway
{
    public class GatewayServer
    {
        private readonly IHost _host;
        private readonly int _port;

        public GatewayServer(int port = 5000)
        {
            _port = port;
            
            _host = Host.CreateDefaultBuilder()
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseUrls($"http://0.0.0.0:{_port}");
                    webBuilder.ConfigureServices(services =>
                    {
                        services.AddSingleton<RouteManager>();
                        services.AddSingleton<ServiceDiscovery>();
                        services.AddSingleton<ILogger<LoggingMiddleware>, Logger<LoggingMiddleware>>();
                    });
                    webBuilder.Configure(app =>
                    {
                        // Middleware pipeline
                        app.UseMiddleware<LoggingMiddleware>();
                        app.UseMiddleware<RateLimitingMiddleware>();
                        app.UseMiddleware<AuthenticationMiddleware>();
                        app.UseMiddleware<RoutingMiddleware>();

                        // Health check endpoint
                        app.Map("/health", healthApp =>
                        {
                            healthApp.Run(async context =>
                            {
                                context.Response.StatusCode = 200;
                                await context.Response.WriteAsync("Gateway is healthy");
                            });
                        });

                        // Admin endpoints
                        app.Map("/admin/routes", adminApp =>
                        {
                            adminApp.Run(async context =>
                            {
                                var routeManager = context.RequestServices.GetService<RouteManager>();
                                var routes = routeManager?.GetAllRoutes() ?? new List<RouteConfig>();
                                
                                context.Response.ContentType = "application/json";
                                await context.Response.WriteAsync(System.Text.Json.JsonSerializer.Serialize(routes));
                            });
                        });
                    });
                })
                .Build();
        }

        public async Task StartAsync()
        {
            await _host.StartAsync();
            Console.WriteLine($"Gateway server started on port {_port}");
        }

        public async Task StopAsync()
        {
            await _host.StopAsync();
            Console.WriteLine("Gateway server stopped");
        }
    }
}
