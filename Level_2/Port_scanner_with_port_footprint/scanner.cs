sharp
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PortScanner
{
    class Program
    {
        // Constants
        private const int DefaultTimeout = 500; // Milliseconds
        private const string HttpFootprint = "HTTP/1.1"; // Check for HTTP response
        private const string FtpFootprint = "220";      // Check for FTP welcome message
        private const string DnsFootprint = "DNS";      // Placeholder (DNS requires more complex handling)
        private const string IrcFootprint = "PING";     // Placeholder (IRC requires more complex handling)

        // Structure to hold scan results
        public struct ScanResult
        {
            public int Port;
            public bool IsOpen;
            public string Service; // Detected service
        }

        static async Task Main(string[] args)
        {
            if (args.Length != 1)
            {
                Console.WriteLine("Usage: PortScanner <hostname/IP>");
                return;
            }

            string host = args[0];
            IPAddress[] addresses;

            try
            {
                addresses = await Dns.GetHostAddressesAsync(host);
            }
            catch (SocketException ex)
            {
                Console.WriteLine($"Error resolving host: {ex.Message}");
                return;
            }

            if (addresses.Length == 0)
            {
                Console.WriteLine("No IP addresses found for the host.");
                return;
            }

            IPAddress ipAddress = addresses[0]; // Use the first IP address
            Console.WriteLine($"Scanning {host} ({ipAddress})...");

            List<ScanResult> results = await ScanPorts(ipAddress, 1, 1024); // Scan common ports

            foreach (var result in results)
            {
                if (result.IsOpen)
                {
                    Console.WriteLine($"Port {result.Port} is open. Service: {result.Service}");
                }
            }

            Console.WriteLine("Scan complete.");
        }

        static async Task<List<ScanResult>> ScanPorts(IPAddress ipAddress, int startPort, int endPort)
        {
            List<ScanResult> scanResults = new List<ScanResult>();
            List<Task<ScanResult>> tasks = new List<Task<ScanResult>>();

            for (int port = startPort; port <= endPort; port++)
            {
                int currentPort = port; // Capture the port for the task

                tasks.Add(Task.Run(async () =>
                {
                    return await ScanPort(ipAddress, currentPort);
                }));
            }

            // Wait for all tasks to complete
            await Task.WhenAll(tasks);

            foreach (var task in tasks)
            {
                scanResults.Add(task.Result);
            }

            return scanResults;
        }

        static async Task<ScanResult> ScanPort(IPAddress ipAddress, int port)
        {
            ScanResult result = new ScanResult { Port = port, IsOpen = false, Service = "Unknown" };

            try
            {
                using (TcpClient client = new TcpClient())
                {
                    // Asynchronous connection with timeout
                    Task connectTask = client.ConnectAsync(ipAddress, port);
                    Task timeoutTask = Task.Delay(DefaultTimeout);

                    Task completedTask = await Task.WhenAny(connectTask, timeoutTask);

                    if (completedTask == timeoutTask)
                    {
                        // Timeout occurred
                        return result; // Port is likely closed or filtered
                    }

                    await connectTask; // Ensure the connection task is awaited to handle exceptions

                    if (client.Connected)
                    {
                        result.IsOpen = true;
                        result.Service = await GetServiceFootprint(client);
                    }
                }
            }
            catch (Exception)
            {
                // Handle exceptions like SocketException (connection refused)
                return result; // Port is likely closed or filtered
            }

            return result;
        }

        static async Task<string> GetServiceFootprint(TcpClient client)
        {
            try
            {
                NetworkStream stream = client.GetStream();
                stream.ReadTimeout = DefaultTimeout;

                // HTTP
                if (TryGetService(stream, "GET / HTTP/1.1\r\nHost: example.com\r\nConnection: close\r\n\r\n", HttpFootprint, out string httpResult))
                {
                    return "HTTP";
                }

                // FTP
                if (TryGetService(stream, "", FtpFootprint, out string ftpResult))
                {
                    return "FTP";
                }

                // DNS (Basic check - needs more robust implementation)
                // This is a placeholder, DNS usually requires sending a DNS query
               /*  if (TryGetService(stream, "TODO: DNS QUERY", DnsFootprint, out string dnsResult))
                {
                    return "DNS";
                }*/

                // IRC (Basic check - needs more robust implementation)
                // This is a placeholder, IRC usually requires sending NICK and USER commands
                /*if (TryGetService(stream, "TODO: IRC COMMANDS", IrcFootprint, out string ircResult))
                {
                    return "IRC";
                }*/
            }
            catch (Exception ex)
            {
                // Handle exceptions during footprinting
                return $"Unknown (Footprint Error: {ex.Message})";
            }

            return "Unknown";
        }

        static bool TryGetService(NetworkStream stream, string request, string footprint, out string response)
        {
            response = null;
            byte[] requestBytes = Encoding.ASCII.GetBytes(request);

            try
            {
                if (!string.IsNullOrEmpty(request))
                {
                    stream.Write(requestBytes, 0, requestBytes.Length);
                    stream.Flush();
                }

                byte[] buffer = new byte[1024];
                int bytesRead = stream.Read(buffer, 0, buffer.Length);
                response = Encoding.ASCII.GetString(buffer, 0, bytesRead);

                return response.Contains(footprint);
            }
            catch (Exception)
            {
                // Ignore exceptions (timeout, etc.)
                return false;
            }
        }
    }
}
