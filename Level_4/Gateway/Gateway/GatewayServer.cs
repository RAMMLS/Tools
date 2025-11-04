using Gateway.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Threading.Tasks;

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
                    });
                    webBuilder.Configure(app =>
                    {
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
                                var routes = routeManager?.GetAllRoutes() ?? new System.Collections.Generic.List<Models.RouteConfig>();
                                
                                context.Response.ContentType = "application/json";
                                await context.Response.WriteAsync(System.Text.Json.JsonSerializer.Serialize(routes));
                            });
                        });

                        // Simple routing for demo
                        app.Run(async context =>
                        {
                            var routeManager = context.RequestServices.GetService<RouteManager>();
                            var path = context.Request.Path;
                            var method = context.Request.Method;

                            context.Response.ContentType = "application/json";
                            
                            if (path == "/api/users")
                            {
                                await context.Response.WriteAsync($"{{\"message\": \"Users from Gateway\", \"gateway\": \"Docker\", \"timestamp\": \"{DateTime.Now:yyyy-MM-dd HH:mm:ss}\"}}");
                            }
                            else if (path == "/api/products")
                            {
                                await context.Response.WriteAsync($"{{\"message\": \"Products from Gateway\", \"gateway\": \"Docker\", \"timestamp\": \"{DateTime.Now:yyyy-MM-dd HH:mm:ss}\"}}");
                            }
                            else
                            {
                                context.Response.StatusCode = 404;
                                await context.Response.WriteAsync($"{{\"error\": \"Route not found: {path}\", \"gateway\": \"Docker\"}}");
                            }
                        });
                    });
                })
                .Build();
        }

        public async Task StartAsync()
        {
            await _host.StartAsync();
            Console.WriteLine($"✅ Gateway server started on port {_port}");
        }

        public async Task StopAsync()
        {
            await _host.StopAsync();
            Console.WriteLine("🛑 Gateway server stopped");
        }
    }
}
