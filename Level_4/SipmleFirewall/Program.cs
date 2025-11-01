using SimpleFirewall;
using SimpleFirewall.Models;

namespace SimpleFirewall
{
    class Program
    {
        private static FirewallEngine? _firewall;

        static void Main(string[] args)
        {
            Console.Title = "Simple Firewall";
            Console.WriteLine("🔥 Simple Firewall Starting...");
            Console.WriteLine("==============================\n");

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

            Console.WriteLine("Firewall stopped.");
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

            Console.Write("Source IP (empty for any): ");
            rule.SourceIP = Console.ReadLine() ?? "";

            Console.Write("Destination IP (empty for any): ");
            rule.DestinationIP = Console.ReadLine() ?? "";

            Console.Write("Source Port (-1 for any): ");
            if (int.TryParse(Console.ReadLine(), out var sourcePort))
                rule.SourcePort = sourcePort;

            Console.Write("Destination Port (-1 for any): ");
            if (int.TryParse(Console.ReadLine(), out var destPort))
                rule.DestinationPort = destPort;

            Console.Write("Protocol (TCP/UDP/ICMP/Any): ");
            if (Enum.TryParse<ProtocolType>(Console.ReadLine(), true, out var protocol))
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

            foreach (var log in logs.TakeLast(20)) // Последние 20 записей
            {
                if (log.Contains("[BLOCK]"))
                    Console.ForegroundColor = ConsoleColor.Red;
                else if (log.Contains("[ALLOW]"))
                    Console.ForegroundColor = ConsoleColor.Green;
                else if (log.Contains("[ERROR]"))
                    Console.ForegroundColor = ConsoleColor.Yellow;
                else
                    Console.ForegroundColor = ConsoleColor.Gray;

                Console.WriteLine(log);
                Console.ResetColor();
            }
        }

        static void OpenWebInterface()
        {
            Console.WriteLine("\n🌐 Opening Web Interface...");
            Console.WriteLine("URL: http://localhost:8080");
            Console.WriteLine("Press Ctrl+C to stop the firewall");

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
