using System;
using System.Text;

namespace ARPPoisoningTool.Core
{
    public class PacketBuilder
    {
        public byte[] BuildARPPacket(string sourceIP, string sourceMAC, string targetIP, string targetMAC)
        {
            // Симуляция построения ARP пакета
            var packetInfo = $"ARP Packet: {sourceIP}({sourceMAC}) -> {targetIP}({targetMAC})";
            return Encoding.UTF8.GetBytes(packetInfo);
        }

        public byte[] BuildEthernetFrame(string sourceMAC, string destinationMAC, byte[] payload)
        {
            // Симуляция построения Ethernet фрейма
            var frameInfo = $"Ethernet Frame: {sourceMAC} -> {destinationMAC}";
            var combined = new byte[Encoding.UTF8.GetBytes(frameInfo).Length + payload.Length];
            
            Buffer.BlockCopy(Encoding.UTF8.GetBytes(frameInfo), 0, combined, 0, frameInfo.Length);
            Buffer.BlockCopy(payload, 0, combined, frameInfo.Length, payload.Length);
            
            return combined;
        }

        public string ParseARPPacket(byte[] packet)
        {
            return Encoding.UTF8.GetString(packet);
        }
    }
}
