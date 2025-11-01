using SimpleFirewall.Models;
using SimpleFirewall.Services;

namespace SimpleFirewall
{
    public class FirewallEngine : IDisposable
    {
        private readonly RuleManager _ruleManager;
        private readonly PacketFilter _packetFilter;
        private readonly LogService _logService;
        private readonly NetworkMonitor _networkMonitor;
        private readonly WebInterface _webInterface;
        private bool _isRunning;

        public FirewallEngine()
        {
            _logService = new LogService();
            _ruleManager = new RuleManager();
            _networkMonitor = new NetworkMonitor(_logService);
            _packetFilter = new PacketFilter(_ruleManager, _logService);
            _webInterface = new WebInterface(_ruleManager, _logService, _networkMonitor);

            SetupEventHandlers();
        }

        private void SetupEventHandlers()
        {
            _ruleManager.RuleAdded += OnRuleAdded;
            _ruleManager.RuleRemoved += OnRuleRemoved;
            _packetFilter.PacketProcessed += OnPacketProcessed;
        }

        public void Start()
        {
            if (_isRunning) return;

            _logService.LogInfo("Starting Simple Firewall...");

            // Загружаем правила по умолчанию
            _ruleManager.LoadDefaultRules();

            // Запускаем компоненты
            _networkMonitor.Start();
            _packetFilter.Start();
            _webInterface.Start();

            _isRunning = true;
            _logService.LogInfo("Simple Firewall started successfully");
        }

        public void Stop()
        {
            if (!_isRunning) return;

            _logService.LogInfo("Stopping Simple Firewall...");

            _webInterface.Stop();
            _packetFilter.Stop();
            _networkMonitor.Stop();

            _isRunning = false;
            _logService.LogInfo("Simple Firewall stopped");
        }

        public void AddRule(FirewallRule rule)
        {
            _ruleManager.AddRule(rule);
        }

        public void RemoveRule(string ruleId)
        {
            _ruleManager.RemoveRule(ruleId);
        }

        public List<FirewallRule> GetRules()
        {
            return _ruleManager.GetAllRules();
        }

        public List<string> GetLogs()
        {
            return _logService.GetRecentLogs();
        }

        private void OnRuleAdded(FirewallRule rule)
        {
            _logService.LogInfo($"Rule added: {rule.Name}");
        }

        private void OnRuleRemoved(FirewallRule rule)
        {
            _logService.LogInfo($"Rule removed: {rule.Name}");
        }

        private void OnPacketProcessed(NetworkPacket packet, RuleAction action)
        {
            // Дополнительная обработка если нужно
        }

        public void Dispose()
        {
            Stop();
            _packetFilter?.Dispose();
            _networkMonitor?.Dispose();
            _webInterface?.Dispose();
            _logService?.Dispose();
        }
    }
}
