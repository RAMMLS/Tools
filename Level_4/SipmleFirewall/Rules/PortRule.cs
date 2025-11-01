using SimpleFirewall.Models;

namespace SimpleFirewall.Rules
{
    public class PortRule : IRule
    {
        public string Name { get; set; } = "Port Rule";
        public int Priority { get; set; } = 90;
        public int SourcePort { get; set; } = -1;
        public int DestinationPort { get; set; } = -1;
        public RuleAction Action { get; set; }
        public RuleDirection Direction { get; set; }

        public bool Evaluate(NetworkPacket packet)
        {
            if (Direction != RuleDirection.Both && packet.Direction != Direction)
                return false;

            bool sourceMatch = SourcePort == -1 || packet.SourcePort == SourcePort;
            bool destMatch = DestinationPort == -1 || packet.DestinationPort == DestinationPort;

            return sourceMatch && destMatch;
        }

        public RuleAction GetAction() => Action;
    }
}
