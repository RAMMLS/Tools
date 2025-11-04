using Gateway;

namespace Gateway
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.Title = "API Gateway";
            Console.WriteLine("🌐 API Gateway Starting...");
            Console.WriteLine("==========================\n");

            var port = 5000;
            if (args.Length > 0 && int.TryParse(args[0], out int customPort))
            {
                port = customPort;
            }

            var gateway = new GatewayServer(port);

            // Обработка Ctrl+C
            Console.CancelKeyPress += (sender, e) =>
            {
                e.Cancel = true;
                Console.WriteLine("\nShutting down gateway...");
                gateway.StopAsync().Wait();
            };

            try
            {
                await gateway.StartAsync();
                
                Console.WriteLine($"Gateway is running on http://localhost:{port}");
                Console.WriteLine("Endpoints:");
                Console.WriteLine($"  Health check: http://localhost:{port}/health");
                Console.WriteLine($"  Admin routes: http://localhost:{port}/admin/routes");
                Console.WriteLine("\nPress Ctrl+C to stop...");

                // Ожидаем завершения
                await Task.Delay(-1);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Gateway error: {ex.Message}");
            }
        }
    }
}
