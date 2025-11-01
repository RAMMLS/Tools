using SimpleFirewall.Models;

namespace SimpleFirewall.Rules
{
    public class ProtocolRule : IRule
    {
        public string Name { get; set; } = "Protocol Rule";
        public int Priority { get; set; } = 80;
        public ProtocolType Protocol { get; set; }
        public RuleAction Action { get; set; }
        public RuleDirection Direction { get; set; }

        public bool Evaluate(NetworkPacket packet)
        {
            if (Direction != RuleDirection.Both && packet.Direction != Direction)
                return false;

            return packet.Protocol == Protocol;
        }

        public RuleAction GetAction() => Action;
    }
}
