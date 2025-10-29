using ChatShared.Enums;

namespace ChatClient.Models
{
    public class Message
    {
        public MessageType Type { get; set; }
        public string Sender { get; set; } = string.Empty;
        public string Recipient { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; }
        public Dictionary<string, string> Metadata { get; set; } = new();

        public override string ToString()
        {
            return Type switch
            {
                MessageType.Text => $"[{Timestamp:HH:mm}] {Sender}: {Content}",
                MessageType.Private => $"[{Timestamp:HH:mm}] {Sender} -> You: {Content}",
                MessageType.System => $"[SYSTEM] {Content}",
                MessageType.UserJoined => $"[SYSTEM] {Sender} joined the chat",
                MessageType.UserLeft => $"[SYSTEM] {Sender} left the chat",
                MessageType.Error => $"[ERROR] {Content}",
                MessageType.File when Metadata.TryGetValue("action", out var action) && action == "request" 
                    => $"[FILE] {Sender} wants to send you: {Content}",
                _ => Content
            };
        }
    }
}
