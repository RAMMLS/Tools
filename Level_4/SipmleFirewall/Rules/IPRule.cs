using System.Net;
using SimpleFirewall.Models;

namespace SimpleFirewall.Rules
{
    public class IPRule : IRule
    {
        public string Name { get; set; } = "IP Rule";
        public int Priority { get; set; } = 100;
        public IPAddress SourceIP { get; set; } = IPAddress.Any;
        public IPAddress DestinationIP { get; set; } = IPAddress.Any;
        public RuleAction Action { get; set; }
        public RuleDirection Direction { get; set; }

        public bool Evaluate(NetworkPacket packet)
        {
            if (Direction != RuleDirection.Both && packet.Direction != Direction)
                return false;

            bool sourceMatch = SourceIP.Equals(IPAddress.Any) || packet.SourceAddress.Equals(SourceIP);
            bool destMatch = DestinationIP.Equals(IPAddress.Any) || packet.DestinationAddress.Equals(DestinationIP);

            return sourceMatch && destMatch;
        }

        public RuleAction GetAction() => Action;
    }
}
