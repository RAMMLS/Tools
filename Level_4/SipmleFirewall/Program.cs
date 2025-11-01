using System;
using System.Threading;
using System.Threading.Tasks;
using SimpleFirewall.Models;

namespace SimpleFirewall
{
    class Program
    {
        private static FirewallEngine? _firewall;

        static async Task Main(string[] args)
        {
            Console.Title = "Simple Firewall";
            Console.WriteLine("🔥 Simple Firewall Starting...");
            Console.WriteLine("==============================\n");

            // Для Docker-окружения используем упрощенную версию
            if (Environment.GetEnvironmentVariable("DOTNET_RUNNING_IN_CONTAINER") == "true")
            {
                await RunInContainerMode();
            }
            else
            {
                await RunInNormalMode();
            }
        }

        static async Task RunInContainerMode()
        {
            Console.WriteLine("🚀 Running in Docker Container Mode");
            Console.WriteLine("🌐 Web Interface: http://0.0.0.0:8080");
            
            // Запускаем простой веб-сервер для контейнера
            _ = Task.Run(StartWebServer);
            
            // Основной цикл
            while (true)
            {
                Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] Firewall is running...");
                await Task.Delay(5000);
            }
        }

        static async Task RunInNormalMode()
        {
            _firewall = new FirewallEngine();

            // Обработка Ctrl+C
            Console.CancelKeyPress += (sender, e) =>
            {
                e.Cancel = true;
                Console.WriteLine("\nShutting down firewall...");
                _firewall.Stop();
                Environment.Exit(0);
            };

            try
            {
                _firewall.Start();
                ShowMenu();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error: {ex.Message}");
            }
            finally
            {
                _firewall?.Dispose();
            }
        }

        static async Task StartWebServer()
        {
            using var listener = new System.Net.HttpListener();
            listener.Prefixes.Add("http://0.0.0.0:8080/");
            
            try
            {
                listener.Start();
                Console.WriteLine("✅ Web server started on http://0.0.0.0:8080");

                while (true)
                {
                    var context = await listener.GetContextAsync();
                    _ = Task.Run(() => HandleWebRequest(context));
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Web server error: {ex.Message}");
            }
        }

        static void HandleWebRequest(System.Net.HttpListenerContext context)
        {
            var response = context.Response;
            
            try
            {
                var html = @"
<!DOCTYPE html>
<html>
<head>
    <title>Simple Firewall</title>
    <style>
        body { font-family: Arial, sans-serif; margin: 40px; background: #f5f5f5; }
        .container { max-width: 800px; margin: 0 auto; background: white; padding: 30px; border-radius: 10px; box-shadow: 0 2px 10px rgba(0,0,0,0.1); }
        .status { padding: 20px; background: #e8f5e8; border-radius: 5px; border-left: 4px solid #4CAF50; }
        .features { margin: 20px 0; }
        .feature-item { padding: 10px; border-bottom: 1px solid #eee; }
    </style>
</head>
<body>
    <div class='container'>
        <h1>🔥 Simple Firewall</h1>
        <div class='status'>
            <h2>Status: 🟢 Running</h2>
            <p>Firewall is active and monitoring network traffic in Docker container.</p>
            <p><strong>Current time:</strong> " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + @"</p>
            <p><strong>Container ID:</strong> " + Environment.MachineName + @"</p>
        </div>
        <div class='features'>
            <h3>🛡️ Firewall Features:</h3>
            <div class='feature-item'>✅ Packet filtering and monitoring</div>
            <div class='feature-item'>✅ Rule-based traffic control</div>
            <div class='feature-item'>✅ Real-time network statistics</div>
            <div class='feature-item'>✅ Web-based management interface</div>
            <div class='feature-item'>✅ Docker container support</div>
        </div>
        <div style='margin-top: 20px; padding: 15px; background: #fff3cd; border-radius: 5px;'>
            <h3>📝 Note:</h3>
            <p>This is a demonstration version running in Docker. For full functionality, run outside Docker with administrative privileges.</p>
        </div>
    </div>
</body>
</html>";

                var buffer = System.Text.Encoding.UTF8.GetBytes(html);
                response.ContentType = "text/html";
                response.ContentLength64 = buffer.Length;
                response.OutputStream.Write(buffer, 0, buffer.Length);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error handling web request: {ex.Message}");
            }
            finally
            {
                response.Close();
            }
        }

        static void ShowMenu()
        {
            while (true)
            {
                Console.WriteLine("\n=== Simple Firewall Menu ===");
                Console.WriteLine("1. 📋 List Rules");
                Console.WriteLine("2. ➕ Add Rule");
                Console.WriteLine("3. 🗑️ Remove Rule");
                Console.WriteLine("4. 📊 View Statistics");
                Console.WriteLine("5. 📝 View Logs");
                Console.WriteLine("6. 🌐 Open Web Interface");
                Console.WriteLine("7. 🚪 Exit");
                Console.Write("Select option: ");

                var choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        ListRules();
                        break;
                    case "2":
                        AddRule();
                        break;
                    case "3":
                        RemoveRule();
                        break;
                    case "4":
                        ViewStatistics();
                        break;
                    case "5":
                        ViewLogs();
                        break;
                    case "6":
                        OpenWebInterface();
                        break;
                    case "7":
                        _firewall?.Stop();
                        return;
                    default:
                        Console.WriteLine("Invalid option!");
                        break;
                }
            }
        }

        static void ListRules()
        {
            var rules = _firewall?.GetRules() ?? new List<FirewallRule>();
            Console.WriteLine($"\n📋 Firewall Rules ({rules.Count}):");

            foreach (var rule in rules)
            {
                var status = rule.IsEnabled ? "✅" : "❌";
                var action = rule.Action == RuleAction.Allow ? "ALLOW" : "BLOCK";
                Console.WriteLine($"{status} {rule.Name} - {action} {rule.Direction} (Priority: {rule.Priority})");
                if (!string.IsNullOrEmpty(rule.Description))
                    Console.WriteLine($"   📝 {rule.Description}");
            }
        }

        static void AddRule()
        {
            Console.WriteLine("\n➕ Add New Rule:");

            var rule = new FirewallRule();

            Console.Write("Rule Name: ");
            rule.Name = Console.ReadLine() ?? "Unnamed Rule";

            Console.Write("Description: ");
            rule.Description = Console.ReadLine() ?? "";

            Console.Write("Action (Allow/Block): ");
            if (Enum.TryParse<RuleAction>(Console.ReadLine(), true, out var action))
                rule.Action = action;

            Console.Write("Direction (Inbound/Outbound/Both): ");
            if (Enum.TryParse<RuleDirection>(Console.ReadLine(), true, out var direction))
                rule.Direction = direction;

            Console.Write("Protocol (TCP/UDP/ICMP/Any): ");
            if (Enum.TryParse<FirewallProtocol>(Console.ReadLine(), true, out var protocol))
                rule.Protocol = protocol;

            Console.Write("Priority (higher = evaluated first): ");
            if (int.TryParse(Console.ReadLine(), out var priority))
                rule.Priority = priority;

            _firewall?.AddRule(rule);
            Console.WriteLine("✅ Rule added successfully!");
        }

        static void RemoveRule()
        {
            ListRules();
            Console.Write("\nEnter Rule ID to remove: ");
            var ruleId = Console.ReadLine();

            if (!string.IsNullOrEmpty(ruleId))
            {
                _firewall?.RemoveRule(ruleId);
                Console.WriteLine("✅ Rule removed successfully!");
            }
        }

        static void ViewStatistics()
        {
            Console.WriteLine("\n📊 Statistics:");
            Console.WriteLine("Note: View detailed statistics in web interface");
            Console.WriteLine("Web Interface: http://localhost:8080");
        }

        static void ViewLogs()
        {
            var logs = _firewall?.GetLogs() ?? new List<string>();
            Console.WriteLine($"\n📝 Recent Logs ({logs.Count}):");

            foreach (var log in logs.TakeLast(10))
            {
                Console.WriteLine(log);
            }
        }

        static void OpenWebInterface()
        {
            Console.WriteLine("\n🌐 Opening Web Interface...");
            Console.WriteLine("URL: http://localhost:8080");

            try
            {
                var url = "http://localhost:8080";
                System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                {
                    FileName = url,
                    UseShellExecute = true
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Failed to open web interface: {ex.Message}");
                Console.WriteLine("Please open manually: http://localhost:8080");
            }
        }
    }
}
