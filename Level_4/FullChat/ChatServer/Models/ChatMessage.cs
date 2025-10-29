using ChatShared.Enums;

namespace ChatServer.Models
{
    public class ChatMessage
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public MessageType Type { get; set; }
        public string Sender { get; set; } = string.Empty;
        public string Recipient { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; } = DateTime.Now;
        public Dictionary<string, string> Metadata { get; set; } = new();

        public override string ToString()
        {
            return Type switch
            {
                MessageType.Text => $"[{Timestamp:HH:mm}] {Sender}: {Content}",
                MessageType.Private => $"[{Timestamp:HH:mm}] {Sender} -> {Recipient}: {Content}",
                MessageType.System => $"[SYSTEM] {Content}",
                MessageType.UserJoined => $"[SYSTEM] {Sender} joined the chat",
                MessageType.UserLeft => $"[SYSTEM] {Sender} left the chat",
                _ => Content
            };
        }
    }
}
