using System.Collections.Concurrent;
using SimpleFirewall.Models;
using SimpleFirewall.Rules;

namespace SimpleFirewall.Services
{
    public class RuleManager
    {
        private readonly ConcurrentDictionary<string, FirewallRule> _rules;
        private readonly List<IRule> _compiledRules;
        private readonly ReaderWriterLockSlim _lock = new();

        public event Action<FirewallRule>? RuleAdded;
        public event Action<FirewallRule>? RuleRemoved;
        public event Action<FirewallRule>? RuleUpdated;

        public RuleManager()
        {
            _rules = new ConcurrentDictionary<string, FirewallRule>();
            _compiledRules = new List<IRule>();
        }

        public void AddRule(FirewallRule rule)
        {
            if (_rules.TryAdd(rule.Id, rule))
            {
                CompileRules();
                RuleAdded?.Invoke(rule);
            }
        }

        public bool RemoveRule(string ruleId)
        {
            if (_rules.TryRemove(ruleId, out var rule))
            {
                CompileRules();
                RuleRemoved?.Invoke(rule);
                return true;
            }
            return false;
        }

        public void UpdateRule(FirewallRule rule)
        {
            if (_rules.TryGetValue(rule.Id, out var existingRule))
            {
                rule.ModifiedAt = DateTime.Now;
                _rules[rule.Id] = rule;
                CompileRules();
                RuleUpdated?.Invoke(rule);
            }
        }

        public FirewallRule? GetRule(string ruleId)
        {
            return _rules.TryGetValue(ruleId, out var rule) ? rule : null;
        }

        public List<FirewallRule> GetAllRules()
        {
            return _rules.Values.OrderByDescending(r => r.Priority).ThenBy(r => r.Name).ToList();
        }

        public List<FirewallRule> GetEnabledRules()
        {
            return _rules.Values.Where(r => r.IsEnabled).OrderByDescending(r => r.Priority).ToList();
        }

        public RuleAction EvaluatePacket(NetworkPacket packet)
        {
            _lock.EnterReadLock();
            try
            {
                foreach (var rule in _compiledRules)
                {
                    if (rule.Evaluate(packet))
                    {
                        return rule.GetAction();
                    }
                }

                // Default action if no rules match
                return RuleAction.Allow;
            }
            finally
            {
                _lock.ExitReadLock();
            }
        }

        private void CompileRules()
        {
            _lock.EnterWriteLock();
            try
            {
                _compiledRules.Clear();

                var enabledRules = GetEnabledRules();
                foreach (var rule in enabledRules)
                {
                    IRule compiledRule = rule.Protocol switch
                    {
                        ProtocolType.TCP or ProtocolType.UDP or ProtocolType.ICMP =>
                            new ProtocolRule
                            {
                                Name = rule.Name,
                                Priority = rule.Priority,
                                Protocol = rule.Protocol,
                                Action = rule.Action,
                                Direction = rule.Direction
                            },
                        _ => new IPRule
                        {
                            Name = rule.Name,
                            Priority = rule.Priority,
                            SourceIP = string.IsNullOrEmpty(rule.SourceIP) ? 
                                System.Net.IPAddress.Any : System.Net.IPAddress.Parse(rule.SourceIP),
                            DestinationIP = string.IsNullOrEmpty(rule.DestinationIP) ? 
                                System.Net.IPAddress.Any : System.Net.IPAddress.Parse(rule.DestinationIP),
                            Action = rule.Action,
                            Direction = rule.Direction
                        }
                    };

                    _compiledRules.Add(compiledRule);
                }

                // Sort by priority (higher priority first)
                _compiledRules.Sort((a, b) => b.Priority.CompareTo(a.Priority));
            }
            finally
            {
                _lock.ExitWriteLock();
            }
        }

        public void LoadDefaultRules()
        {
            // Блокировка известных вредоносных портов
            var blockPorts = new[] { 135, 137, 138, 139, 445, 1433, 1434, 3389 };
            foreach (var port in blockPorts)
            {
                AddRule(new FirewallRule
                {
                    Name = $"Block Port {port}",
                    Description = $"Block incoming connections on port {port}",
                    Action = RuleAction.Block,
                    Direction = RuleDirection.Inbound,
                    DestinationPort = port,
                    Protocol = ProtocolType.Any,
                    Priority = 1000
                });
            }

            // Разрешение основных портов
            var allowPorts = new[] { 80, 443, 53 };
            foreach (var port in allowPorts)
            {
                AddRule(new FirewallRule
                {
                    Name = $"Allow Port {port}",
                    Description = $"Allow outgoing connections on port {port}",
                    Action = RuleAction.Allow,
                    Direction = RuleDirection.Outbound,
                    DestinationPort = port,
                    Protocol = ProtocolType.Any,
                    Priority = 10
                });
            }
        }
    }
}
