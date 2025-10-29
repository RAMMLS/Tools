using ChatServer.Services;

namespace ChatServer
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.Title = "Chat Server";
            Console.WriteLine("=== Chat Server Starting ===");

            var port = 5000;
            if (args.Length > 0 && int.TryParse(args[0], out int customPort))
            {
                port = customPort;
            }

            var chatService = new ChatService(port);

            // Handle Ctrl+C gracefully
            Console.CancelKeyPress += (sender, e) =>
            {
                e.Cancel = true;
                Console.WriteLine("\nShutting down server...");
                chatService.Stop();
            };

            try
            {
                await chatService.StartAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Server error: {ex.Message}");
            }

            Console.WriteLine("Server stopped.");
        }
    }
}
