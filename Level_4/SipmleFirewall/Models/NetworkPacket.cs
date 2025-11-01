using System.Net;

namespace SimpleFirewall.Models
{
    public class NetworkPacket
    {
        public IPAddress SourceAddress { get; set; } = IPAddress.Any;
        public IPAddress DestinationAddress { get; set; } = IPAddress.Any;
        public int SourcePort { get; set; }
        public int DestinationPort { get; set; }
        public ProtocolType Protocol { get; set; }
        public RuleDirection Direction { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.Now;
        public long Size { get; set; }
        public string ProcessName { get; set; } = string.Empty;
        public string Interface { get; set; } = string.Empty;

        public bool IsMatch(FirewallRule rule)
        {
            // Проверка направления
            if (rule.Direction != RuleDirection.Both && rule.Direction != Direction)
                return false;

            // Проверка протокола
            if (rule.Protocol != ProtocolType.Any && rule.Protocol != Protocol)
                return false;

            // Проверка IP адресов
            if (!string.IsNullOrEmpty(rule.SourceIP) && !IsIPMatch(SourceAddress, rule.SourceIP))
                return false;

            if (!string.IsNullOrEmpty(rule.DestinationIP) && !IsIPMatch(DestinationAddress, rule.DestinationIP))
                return false;

            // Проверка портов
            if (rule.SourcePort != -1 && rule.SourcePort != SourcePort)
                return false;

            if (rule.DestinationPort != -1 && rule.DestinationPort != DestinationPort)
                return false;

            return true;
        }

        private bool IsIPMatch(IPAddress packetIP, string ruleIP)
        {
            if (ruleIP.Contains('/'))
            {
                // CIDR notation
                var parts = ruleIP.Split('/');
                if (parts.Length == 2 && IPAddress.TryParse(parts[0], out var networkIP) && int.TryParse(parts[1], out var maskBits))
                {
                    return IsInRange(packetIP, networkIP, maskBits);
                }
            }
            else if (ruleIP.Contains('-'))
            {
                // IP range
                var parts = ruleIP.Split('-');
                if (parts.Length == 2 && IPAddress.TryParse(parts[0], out var startIP) && IPAddress.TryParse(parts[1], out var endIP))
                {
                    return IsInRange(packetIP, startIP, endIP);
                }
            }
            else
            {
                // Single IP
                return IPAddress.TryParse(ruleIP, out var singleIP) && packetIP.Equals(singleIP);
            }

            return false;
        }

        private bool IsInRange(IPAddress ip, IPAddress network, int maskBits)
        {
            // Simplified CIDR check
            var ipBytes = ip.GetAddressBytes();
            var networkBytes = network.GetAddressBytes();
            
            if (ipBytes.Length != networkBytes.Length) return false;

            for (int i = 0; i < ipBytes.Length; i++)
            {
                var mask = (i < maskBits / 8) ? (byte)0xFF : (byte)0;
                if ((ipBytes[i] & mask) != (networkBytes[i] & mask))
                    return false;
            }

            return true;
        }

        private bool IsInRange(IPAddress ip, IPAddress start, IPAddress end)
        {
            var ipBytes = ip.GetAddressBytes();
            var startBytes = start.GetAddressBytes();
            var endBytes = end.GetAddressBytes();

            for (int i = 0; i < ipBytes.Length; i++)
            {
                if (ipBytes[i] < startBytes[i] || ipBytes[i] > endBytes[i])
                    return false;
            }

            return true;
        }
    }
}
