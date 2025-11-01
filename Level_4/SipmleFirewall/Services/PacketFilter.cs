using System.Net;
using System.Net.Sockets;
using System.Net.NetworkInformation;
using SimpleFirewall.Models;

namespace SimpleFirewall.Services
{
    public class PacketFilter : IDisposable
    {
        private readonly RuleManager _ruleManager;
        private readonly LogService _logService;
        private readonly List<Socket> _sockets;
        private bool _isRunning;
        private readonly Thread _captureThread;

        public event Action<NetworkPacket, RuleAction>? PacketProcessed;

        public PacketFilter(RuleManager ruleManager, LogService logService)
        {
            _ruleManager = ruleManager;
            _logService = logService;
            _sockets = new List<Socket>();
            _captureThread = new Thread(CapturePackets);
        }

        public void Start()
        {
            if (_isRunning) return;

            _isRunning = true;
            
            try
            {
                // Получаем все сетевые интерфейсы
                var interfaces = NetworkInterface.GetAllNetworkInterfaces()
                    .Where(nic => nic.OperationalStatus == OperationalStatus.Up &&
                                 nic.NetworkInterfaceType != NetworkInterfaceType.Loopback);

                foreach (var nic in interfaces)
                {
                    try
                    {
                        var socket = new Socket(AddressFamily.InterNetwork, SocketType.Raw, ProtocolType.IP);
                        
                        // Привязываем сокет к интерфейсу
                        var localEndPoint = new IPEndPoint(IPAddress.Any, 0);
                        socket.Bind(localEndPoint);
                        
                        // Включаем получение входящих пакетов
                        socket.IOControl(IOControlCode.ReceiveAll, new byte[] { 1, 0, 0, 0 }, new byte[] { 0, 0, 0, 0 });
                        
                        _sockets.Add(socket);
                        
                        _logService.LogInfo($"Started monitoring interface: {nic.Name}");
                    }
                    catch (Exception ex)
                    {
                        _logService.LogError($"Failed to monitor interface {nic.Name}: {ex.Message}");
                    }
                }

                _captureThread.Start();
                _logService.LogInfo("Packet filter started successfully");
            }
            catch (Exception ex)
            {
                _logService.LogError($"Failed to start packet filter: {ex.Message}");
            }
        }

        public void Stop()
        {
            _isRunning = false;
            
            foreach (var socket in _sockets)
            {
                try
                {
                    socket.Close();
                }
                catch
                {
                    // Игнорируем ошибки при закрытии
                }
            }
            _sockets.Clear();

            _logService.LogInfo("Packet filter stopped");
        }

        private void CapturePackets()
        {
            var buffers = _sockets.ToDictionary(s => s, s => new byte[4096]);
            var endpoints = _sockets.ToDictionary(s => s, s => new IPEndPoint(IPAddress.Any, 0));

            while (_isRunning)
            {
                foreach (var socket in _sockets.ToArray())
                {
                    try
                    {
                        if (socket.Available > 0)
                        {
                            var buffer = buffers[socket];
                            var endPoint = endpoints[socket] as EndPoint;
                            
                            int bytesRead = socket.ReceiveFrom(buffer, ref endPoint);
                            if (bytesRead > 0)
                            {
                                ProcessPacket(buffer, bytesRead, socket);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        if (_isRunning)
                        {
                            _logService.LogError($"Error capturing packet: {ex.Message}");
                        }
                    }
                }

                Thread.Sleep(10); // Небольшая пауза чтобы не грузить CPU
            }
        }

        private void ProcessPacket(byte[] buffer, int length, Socket socket)
        {
            try
            {
                var packet = ParseIPPacket(buffer, length);
                if (packet != null)
                {
                    var action = _ruleManager.EvaluatePacket(packet);
                    
                    // Логируем действие
                    _logService.LogPacket(packet, action);
                    
                    PacketProcessed?.Invoke(packet, action);

                    // Здесь должна быть реальная блокировка пакета
                    // В реальной реализации это потребует драйвер уровня ядра
                    SimulatePacketBlocking(packet, action);
                }
            }
            catch (Exception ex)
            {
                _logService.LogError($"Error processing packet: {ex.Message}");
            }
        }

        private NetworkPacket? ParseIPPacket(byte[] buffer, int length)
        {
            if (length < 20) return null; // Минимальный размер IP заголовка

            byte version = (byte)(buffer[0] >> 4);
            if (version != 4) return null; // Только IPv4

            int headerLength = (buffer[0] & 0x0F) * 4;
            byte protocol = buffer[9];

            var sourceIP = new IPAddress(BitConverter.ToUInt32(buffer, 12));
            var destIP = new IPAddress(BitConverter.ToUInt32(buffer, 16));

            int sourcePort = 0;
            int destinationPort = 0;

            // Парсим порты для TCP/UDP
            if (protocol == 6 || protocol == 17) // TCP или UDP
            {
                if (length >= headerLength + 4)
                {
                    sourcePort = (buffer[headerLength] << 8) | buffer[headerLength + 1];
                    destinationPort = (buffer[headerLength + 2] << 8) | buffer[headerLength + 3];
                }
            }

            return new NetworkPacket
            {
                SourceAddress = sourceIP,
                DestinationAddress = destIP,
                SourcePort = sourcePort,
                DestinationPort = destinationPort,
                Protocol = protocol switch
                {
                    1 => ProtocolType.ICMP,
                    6 => ProtocolType.TCP,
                    17 => ProtocolType.UDP,
                    _ => ProtocolType.Any
                },
                Direction = DetermineDirection(sourceIP, destIP),
                Size = length
            };
        }

        private RuleDirection DetermineDirection(IPAddress source, IPAddress destination)
        {
            // Упрощенная логика определения направления
            // В реальной системе нужно учитывать сетевые интерфейсы и маршрутизацию
            if (source.ToString().StartsWith("192.168.") || source.ToString().StartsWith("10."))
                return RuleDirection.Outbound;
            
            return RuleDirection.Inbound;
        }

        private void SimulatePacketBlocking(NetworkPacket packet, RuleAction action)
        {
            // В реальной системе здесь будет код для реальной блокировки пакета
            // Это требует драйвера уровня ядра или использования Windows Filtering Platform
            
            if (action == RuleAction.Block)
            {
                _logService.LogWarning($"BLOCKED: {packet.SourceAddress}:{packet.SourcePort} -> {packet.DestinationAddress}:{packet.DestinationPort} ({packet.Protocol})");
            }
        }

        public void Dispose()
        {
            Stop();
        }
    }
}
