using System;
using System.Collections.Generic;
using System.Net;
using System.Net.NetworkInformation;
using System.Threading;
using System.Threading.Tasks;
using ARPPoisoningTool.Utilities;
using ARPPoisoningTool.Models;

namespace ARPPoisoningTool.Core
{
    public class ARPPoisoner : IDisposable
    {
        private readonly Logger _logger;
        private Timer _poisonTimer;
        private bool _isRunning;
        private int _packetsSent;
        private DateTime _startTime;

        public ARPPoisoner(Logger logger)
        {
            _logger = logger;
            _isRunning = false;
        }

        public void Start(string targetIP, string gatewayIP, string interfaceName)
        {
            if (_isRunning) return;

            _isRunning = true;
            _packetsSent = 0;
            _startTime = DateTime.Now;

            _logger.LogInfo($"Starting ARP poisoning: {targetIP} <-> {gatewayIP}");

            // Симуляция отправки ARP пакетов
            _poisonTimer = new Timer(async _ =>
            {
                if (_isRunning)
                {
                    await SendARPPacket(targetIP, gatewayIP);
                    await SendARPPacket(gatewayIP, targetIP);
                    _packetsSent += 2;
                }
            }, null, TimeSpan.Zero, TimeSpan.FromSeconds(2));

            _logger.LogInfo("ARP poisoning started successfully");
        }

        private async Task SendARPPacket(string targetIP, string spoofedIP)
        {
            try
            {
                // Симуляция отправки ARP пакета
                await Task.Delay(100);
                
                var message = $"ARP Reply: {spoofedIP} is at {GenerateFakeMAC()} -> {targetIP}";
                _logger.LogInfo(message);
                
                Console.WriteLine($"📤 {message}");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to send ARP packet: {ex.Message}");
            }
        }

        public void Stop()
        {
            if (!_isRunning) return;

            _isRunning = false;
            _poisonTimer?.Dispose();
            
            var duration = DateTime.Now - _startTime;
            _logger.LogInfo($"ARP poisoning stopped. Duration: {duration}, Packets sent: {_packetsSent}");
        }

        public PoisoningStatistics GetStatistics()
        {
            return new PoisoningStatistics
            {
                PacketsSent = _packetsSent,
                TargetsPoisoned = _isRunning ? 2 : 0,
                Duration = DateTime.Now - _startTime
            };
        }

        private string GenerateFakeMAC()
        {
            var random = new Random();
            var bytes = new byte[6];
            random.NextBytes(bytes);
            return string.Join(":", Array.ConvertAll(bytes, b => b.ToString("X2")));
        }

        public void Dispose()
        {
            Stop();
            _poisonTimer?.Dispose();
        }
    }

    public class PoisoningStatistics
    {
        public int PacketsSent { get; set; }
        public int TargetsPoisoned { get; set; }
        public TimeSpan Duration { get; set; }
    }
}
