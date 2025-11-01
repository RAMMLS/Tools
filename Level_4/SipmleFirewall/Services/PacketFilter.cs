using System.Net;
using System.Net.Sockets;
using System.Net.NetworkInformation;
using SimpleFirewall.Models;

namespace SimpleFirewall.Services
{
    public class PacketFilter : IDisposable
    {
        private readonly RuleManager _ruleManager;
        private readonly LogService _logService;
        private readonly List<Socket> _sockets;
        private bool _isRunning;
        private readonly Thread _captureThread;

        public event Action<NetworkPacket, RuleAction>? PacketProcessed;

        public PacketFilter(RuleManager ruleManager, LogService logService)
        {
            _ruleManager = ruleManager;
            _logService = logService;
            _sockets = new List<Socket>();
            _captureThread = new Thread(CapturePackets);
        }

        public void Start()
        {
            if (_isRunning) return;

            _isRunning = true;
            
            try
            {
                // Для демонстрации - симуляция работы без реального захвата пакетов
                _logService.LogInfo("Starting packet filter in simulation mode...");
                _captureThread.Start();
                _logService.LogInfo("Packet filter started successfully");
            }
            catch (Exception ex)
            {
                _logService.LogError($"Failed to start packet filter: {ex.Message}");
            }
        }

        public void Stop()
        {
            _isRunning = false;
            _logService.LogInfo("Packet filter stopped");
        }

        private void CapturePackets()
        {
            var random = new Random();
            
            while (_isRunning)
            {
                try
                {
                    // Симуляция сетевого трафика для демонстрации
                    SimulateNetworkTraffic(random);
                    Thread.Sleep(2000); // Пауза между генерацией пакетов
                }
                catch (Exception ex)
                {
                    if (_isRunning)
                    {
                        _logService.LogError($"Error in packet capture: {ex.Message}");
                    }
                }
            }
        }

        private void SimulateNetworkTraffic(Random random)
        {
            // Генерируем случайные пакеты для демонстрации
            var protocols = new[] { FirewallProtocol.TCP, FirewallProtocol.UDP, FirewallProtocol.ICMP };
            var directions = new[] { RuleDirection.Inbound, RuleDirection.Outbound };
            var ports = new[] { 80, 443, 22, 3389, 53, 8080 };

            for (int i = 0; i < 3; i++) // Генерируем 3 пакета за раз
            {
                var packet = new NetworkPacket
                {
                    SourceAddress = IPAddress.Parse($"192.168.1.{random.Next(1, 255)}"),
                    DestinationAddress = IPAddress.Parse($"10.0.0.{random.Next(1, 255)}"),
                    SourcePort = random.Next(1000, 65535),
                    DestinationPort = ports[random.Next(ports.Length)],
                    Protocol = protocols[random.Next(protocols.Length)],
                    Direction = directions[random.Next(directions.Length)],
                    Size = random.Next(64, 1500)
                };

                var action = _ruleManager.EvaluatePacket(packet);
                _logService.LogPacket(packet, action);
                PacketProcessed?.Invoke(packet, action);
            }
        }

        public void Dispose()
        {
            Stop();
        }
    }
}
