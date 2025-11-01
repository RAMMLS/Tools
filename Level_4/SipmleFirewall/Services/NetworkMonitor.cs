using System.Net.NetworkInformation;
using SimpleFirewall.Models;

namespace SimpleFirewall.Services
{
    public class NetworkMonitor : IDisposable
    {
        private readonly LogService _logService;
        private Timer? _statsTimer;
        private readonly List<TrafficStatistic> _statistics;
        private readonly object _lock = new();

        public event Action<TrafficStatistic>? StatisticsUpdated;

        public NetworkMonitor(LogService logService)
        {
            _logService = logService;
            _statistics = new List<TrafficStatistic>();
        }

        public void Start()
        {
            _statsTimer = new Timer(UpdateStatistics, null, TimeSpan.Zero, TimeSpan.FromSeconds(5));
            _logService.LogInfo("Network monitor started");
        }

        public void Stop()
        {
            _statsTimer?.Dispose();
            _statsTimer = null;
            _logService.LogInfo("Network monitor stopped");
        }

        private void UpdateStatistics(object? state)
        {
            try
            {
                var interfaces = NetworkInterface.GetAllNetworkInterfaces()
                    .Where(nic => nic.OperationalStatus == OperationalStatus.Up);

                foreach (var nic in interfaces)
                {
                    var stats = nic.GetIPStatistics();
                    
                    var statistic = new TrafficStatistic
                    {
                        RuleName = $"Interface: {nic.Name}",
                        BytesAllowed = (long)stats.BytesReceived + (long)stats.BytesSent,
                        PacketsAllowed = (long)stats.UnicastPacketsReceived + (long)stats.UnicastPacketsSent
                    };

                    lock (_lock)
                    {
                        var existing = _statistics.FirstOrDefault(s => s.RuleName == statistic.RuleName);
                        if (existing != null)
                        {
                            _statistics.Remove(existing);
                        }
                        _statistics.Add(statistic);
                    }

                    StatisticsUpdated?.Invoke(statistic);
                }
            }
            catch (Exception ex)
            {
                _logService.LogError($"Error updating statistics: {ex.Message}");
            }
        }

        public List<TrafficStatistic> GetStatistics()
        {
            lock (_lock)
            {
                return new List<TrafficStatistic>(_statistics);
            }
        }

        public void Dispose()
        {
            Stop();
        }
    }
}
