using System.Net;

namespace SimpleFirewall.Models
{
    public enum RuleAction
    {
        Allow,
        Block
    }

    public enum RuleDirection
    {
        Inbound,
        Outbound,
        Both
    }

    // Изменяем имя enum чтобы избежать конфликта
    public enum FirewallProtocol
    {
        TCP,
        UDP,
        ICMP,
        Any
    }

    public class FirewallRule
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public RuleAction Action { get; set; }
        public RuleDirection Direction { get; set; }
        public bool IsEnabled { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime? ModifiedAt { get; set; }
        
        // Условия правила
        public string SourceIP { get; set; } = string.Empty;
        public string DestinationIP { get; set; } = string.Empty;
        public int SourcePort { get; set; } = -1;
        public int DestinationPort { get; set; } = -1;
        public FirewallProtocol Protocol { get; set; } = FirewallProtocol.Any;
        
        // Дополнительные условия
        public string ApplicationPath { get; set; } = string.Empty;
        public string InterfaceName { get; set; } = string.Empty;
        
        public int Priority { get; set; } = 100;

        public override string ToString()
        {
            return $"{Name} ({Action} {Direction})";
        }
    }
}
