using System.Net.Sockets;

namespace ChatServer.Models
{
    public class User
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string Username { get; set; } = string.Empty;
        public TcpClient Client { get; set; }
        public StreamWriter Writer { get; set; }
        public StreamReader Reader { get; set; }
        public DateTime ConnectedAt { get; set; } = DateTime.Now;
        public bool IsOnline { get; set; } = true;

        public User(TcpClient client)
        {
            Client = client;
            var stream = client.GetStream();
            Reader = new StreamReader(stream);
            Writer = new StreamWriter(stream) { AutoFlush = true };
        }

        public void Disconnect()
        {
            try
            {
                IsOnline = false;
                Reader?.Close();
                Writer?.Close();
                Client?.Close();
            }
            catch
            {
                // Ignore errors during disconnect
            }
        }
    }
}
