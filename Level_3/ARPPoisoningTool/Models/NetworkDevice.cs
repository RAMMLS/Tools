using System.Net.NetworkInformation;

namespace ARPPoisoningTool.Models
{
    public class NetworkDevice
    {
        public string IPAddress { get; set; }
        public string MACAddress { get; set; }
        public string Hostname { get; set; }
        public string Vendor { get; set; }
        public bool IsOnline { get; set; } = true;
        public PhysicalAddress PhysicalAddress => PhysicalAddress.Parse(MACAddress?.Replace(":", "-"));

        public override string ToString()
        {
            return $"{IPAddress} ({MACAddress}) - {Hostname}";
        }
    }
}
