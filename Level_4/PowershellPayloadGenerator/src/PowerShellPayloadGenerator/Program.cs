using System;
using PowerShellPayloadGenerator.Models;
using PowerShellPayloadGenerator.Services;
using PowerShellPayloadGenerator.Models;
namespace PowerShellPayloadGenerator
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("=== PowerShell Payload Generator ===");
            Console.WriteLine("Educational Purposes Only!\n");
            
            if (args.Length == 0)
            {
                ShowHelp();
                return;
            }

            try
            {
                var config = ParseArguments(args);
                var service = new PayloadService();
                
                string payload = service.GeneratePayload(config);
                Console.WriteLine($"\nGenerated Payload:\n{new string('-', 50)}");
                Console.WriteLine(payload);
                Console.WriteLine(new string('-', 50));
                
                if (config.Validate)
                {
                    Console.WriteLine("\n[INFO] Payload validation passed");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] {ex.Message}");
                Environment.Exit(1);
            }
        }

        private static PayloadConfig ParseArguments(string[] args)
        {
            var config = new PayloadConfig();
            
            for (int i = 0; i < args.Length; i++)
            {
                switch (args[i])
                {
                    case "--type":
                        config.Type = args[++i];
                        break;
                    case "--ip":
                        config.IpAddress = args[++i];
                        break;
                    case "--port":
                        config.Port = int.Parse(args[++i]);
                        break;
                    case "--encode":
                        config.Encode = true;
                        break;
                    case "--validate":
                        config.Validate = true;
                        break;
                }
            }
            
            return config;
        }

        private static void ShowHelp()
        {
            Console.WriteLine("Usage:");
            Console.WriteLine("  --type      Payload type [ReverseShell, Download, Command]");
            Console.WriteLine("  --ip        Target IP address");
            Console.WriteLine("  --port      Target port");
            Console.WriteLine("  --encode    Encode payload in Base64");
            Console.WriteLine("  --validate  Validate payload syntax");
            Console.WriteLine("\nExample:");
            Console.WriteLine("  dotnet run -- --type ReverseShell --ip 127.0.0.1 --port 8080");
        }
    }
}
