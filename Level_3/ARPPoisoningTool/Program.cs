using System;
using System.Threading;
using System.Threading.Tasks;
using ARPPoisoningTool.Core;
using ARPPoisoningTool.Utilities;
using ARPPoisoningTool.Config;

namespace ARPPoisoningTool
{
    class Program
    {
        private static ARPPoisoner _poisoner;
        private static Logger _logger;
        private static bool _isRunning = false;

        static async Task Main(string[] args)
        {
            Console.WriteLine("🛡️ ARP Poisoning Tool - Educational Purpose Only");
            Console.WriteLine("=================================================\n");

            _logger = new Logger("arp_log.txt");
            _poisoner = new ARPPoisoner(_logger);

            try
            {
                await ShowMenu();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Critical error: {ex.Message}");
                Console.WriteLine($"❌ Error: {ex.Message}");
            }
            finally
            {
                _poisoner?.Stop();
                _logger?.Dispose();
            }
        }

        static async Task ShowMenu()
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine("ARP Poisoning Tool Menu:");
                Console.WriteLine("1. 📡 Scan Network");
                Console.WriteLine("2. 🎯 Start ARP Poisoning");
                Console.WriteLine("3. ⏹️ Stop ARP Poisoning");
                Console.WriteLine("4. 📊 Show Statistics");
                Console.WriteLine("5. 📁 View Logs");
                Console.WriteLine("6. 🚪 Exit");
                Console.Write("\nSelect option: ");

                var choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        await ScanNetwork();
                        break;
                    case "2":
                        StartPoisoning();
                        break;
                    case "3":
                        StopPoisoning();
                        break;
                    case "4":
                        ShowStatistics();
                        break;
                    case "5":
                        ViewLogs();
                        break;
                    case "6":
                        return;
                    default:
                        Console.WriteLine("Invalid option!");
                        Thread.Sleep(1000);
                        break;
                }
            }
        }

        static async Task ScanNetwork()
        {
            Console.Write("Enter network range (e.g., 192.168.1.0/24): ");
            var range = Console.ReadLine();
            
            var scanner = new NetworkScanner(_logger);
            var devices = await scanner.ScanAsync(range);
            
            Console.WriteLine($"\n📡 Found {devices.Count} devices:");
            foreach (var device in devices)
            {
                Console.WriteLine($"   {device.IPAddress} - {device.MACAddress} - {device.Hostname}");
            }
            
            Console.WriteLine("\nPress any key to continue...");
            Console.ReadKey();
        }

        static void StartPoisoning()
        {
            Console.Write("Target IP: ");
            var targetIp = Console.ReadLine();
            
            Console.Write("Gateway IP: ");
            var gatewayIp = Console.ReadLine();
            
            Console.Write("Network Interface: ");
            var interfaceName = Console.ReadLine();

            _poisoner.Start(targetIp, gatewayIp, interfaceName);
            _isRunning = true;
            
            Console.WriteLine($"\n🎯 ARP Poisoning started: {targetIp} <-> {gatewayIp}");
            Thread.Sleep(2000);
        }

        static void StopPoisoning()
        {
            _poisoner.Stop();
            _isRunning = false;
            Console.WriteLine("⏹️ ARP Poisoning stopped");
            Thread.Sleep(1000);
        }

        static void ShowStatistics()
        {
            if (_poisoner != null)
            {
                var stats = _poisoner.GetStatistics();
                Console.WriteLine($"\n📊 Statistics:");
                Console.WriteLine($"   Packets Sent: {stats.PacketsSent}");
                Console.WriteLine($"   Targets Poisoned: {stats.TargetsPoisoned}");
                Console.WriteLine($"   Duration: {stats.Duration}");
            }
            
            Console.WriteLine("\nPress any key to continue...");
            Console.ReadKey();
        }

        static void ViewLogs()
        {
            var logs = _logger.GetRecentLogs(10);
            Console.WriteLine("\n📁 Recent Logs:");
            foreach (var log in logs)
            {
                Console.WriteLine($"   {log}");
            }
            Console.WriteLine("\nPress any key to continue...");
            Console.ReadKey();
        }
    }
}
