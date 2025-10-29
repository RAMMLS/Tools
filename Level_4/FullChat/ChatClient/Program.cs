using ChatClient.Models;
using ChatClient.Services;
using ChatShared.Enums;

namespace ChatClient
{
    class Program
    {
        private static NetworkClient? _client;
        private static string _username = string.Empty;
        private static List<string> _onlineUsers = new();

        static async Task Main(string[] args)
        {
            Console.Title = "Chat Client";
            Console.WriteLine("=== Chat Client ===");

            var server = "localhost";
            var port = 5000;

            if (args.Length >= 2)
            {
                server = args[0];
                port = int.Parse(args[1]);
            }

            _client = new NetworkClient();
            _client.MessageReceived += OnMessageReceived;
            _client.ConnectionLost += OnConnectionLost;

            if (!await _client.ConnectAsync(server, port))
            {
                Console.WriteLine("Failed to connect to server. Press any key to exit.");
                Console.ReadKey();
                return;
            }

            await RunChatAsync();
        }

        private static async Task RunChatAsync()
        {
            try
            {
                while (true)
                {
                    Console.Write("> ");
                    var input = Console.ReadLine();

                    if (string.IsNullOrWhiteSpace(input))
                        continue;

                    if (input.Equals("/exit", StringComparison.OrdinalIgnoreCase))
                        break;

                    await ProcessInputAsync(input);
                }
            }
            finally
            {
                _client?.Disconnect();
                Console.WriteLine("Disconnected from server.");
            }
        }

        private static async Task ProcessInputAsync(string input)
        {
            if (input.StartsWith("/private "))
            {
                var parts = input.Split(' ', 3);
                if (parts.Length >= 3)
                {
                    await _client!.SendMessageAsync(parts[2], parts[1]);
                }
                else
                {
                    Console.WriteLine("Usage: /private <username> <message>");
                }
            }
            else if (input.StartsWith("/file "))
            {
                var parts = input.Split(' ', 3);
                if (parts.Length >= 3)
                {
                    await _client!.SendFileAsync(parts[2], parts[1]);
                }
                else
                {
                    Console.WriteLine("Usage: /file <username> <filepath>");
                }
            }
            else if (input.Equals("/list", StringComparison.OrdinalIgnoreCase))
            {
                Console.WriteLine("Online users: " + string.Join(", ", _onlineUsers));
            }
            else if (input.Equals("/help", StringComparison.OrdinalIgnoreCase))
            {
                ShowHelp();
            }
            else
            {
                await _client!.SendMessageAsync(input);
            }
        }

        private static void OnMessageReceived(Message message)
        {
            // Handle different message types
            switch (message.Type)
            {
                case MessageType.UserList:
                    _onlineUsers = System.Text.Json.JsonSerializer.Deserialize<List<string>>(message.Content) ?? new List<string>();
                    break;

                case MessageType.File when message.Metadata.TryGetValue("action", out var action) && action == "request":
                    HandleFileTransferRequest(message);
                    break;

                default:
                    Console.WriteLine(message.ToString());
                    break;
            }
        }

        private static void HandleFileTransferRequest(Message message)
        {
            Console.WriteLine(message.ToString());
            Console.Write("Accept file transfer? (y/n): ");
            var response = Console.ReadLine()?.Trim().ToLower();
            if (response == "y" || response == "yes")
            {
                if (message.Metadata.TryGetValue("transferId", out var transferId) &&
                    message.Metadata.TryGetValue("fileName", out var fileName) &&
                    message.Metadata.TryGetValue("fileSize", out var fileSizeStr) &&
                    long.TryParse(fileSizeStr, out var fileSize))
                {
                    _ = _client!.AcceptFileTransferAsync(transferId, fileName, fileSize);
                }
            }
        }

        private static void OnConnectionLost(string reason)
        {
            Console.WriteLine($"\n{reason}");
            Environment.Exit(1);
        }

        private static void ShowHelp()
        {
            Console.WriteLine("Available commands:");
            Console.WriteLine("  <message>          - Send public message");
            Console.WriteLine("  /private <user> <msg> - Send private message");
            Console.WriteLine("  /file <user> <path>   - Send file to user");
            Console.WriteLine("  /list              - Show online users");
            Console.WriteLine("  /help              - Show this help");
            Console.WriteLine("  /exit              - Exit chat");
        }
    }
}
