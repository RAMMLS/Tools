using System.Text;
using System.Text.Json;
using ChatServer.Models;
using ChatShared.Enums;

namespace ChatServer.Protocols
{
    public static class MessageProtocol
    {
        public static string SerializeMessage(ChatMessage message)
        {
            var messageData = new
            {
                type = message.Type.ToString(),
                id = message.Id,
                sender = message.Sender,
                recipient = message.Recipient,
                content = message.Content,
                timestamp = message.Timestamp,
                metadata = message.Metadata
            };

            return JsonSerializer.Serialize(messageData) + "\n";
        }

        public static ChatMessage? DeserializeMessage(string json)
        {
            try
            {
                var messageData = JsonSerializer.Deserialize<JsonElement>(json);
                
                var message = new ChatMessage
                {
                    Type = Enum.Parse<MessageType>(messageData.GetProperty("type").GetString() ?? "Text"),
                    Id = messageData.GetProperty("id").GetString() ?? Guid.NewGuid().ToString(),
                    Sender = messageData.GetProperty("sender").GetString() ?? string.Empty,
                    Recipient = messageData.GetProperty("recipient").GetString() ?? string.Empty,
                    Content = messageData.GetProperty("content").GetString() ?? string.Empty,
                    Timestamp = messageData.GetProperty("timestamp").GetDateTime()
                };

                // Parse metadata if exists
                if (messageData.TryGetProperty("metadata", out var metadata))
                {
                    foreach (var prop in metadata.EnumerateObject())
                    {
                        message.Metadata[prop.Name] = prop.Value.GetString() ?? string.Empty;
                    }
                }

                return message;
            }
            catch
            {
                return null;
            }
        }

        public static string CreateSystemMessage(string content)
        {
            return SerializeMessage(new ChatMessage
            {
                Type = MessageType.System,
                Sender = "SYSTEM",
                Content = content
            });
        }

        public static string CreateUserListMessage(List<string> users)
        {
            return SerializeMessage(new ChatMessage
            {
                Type = MessageType.UserList,
                Sender = "SYSTEM",
                Content = JsonSerializer.Serialize(users)
            });
        }

        public static string CreateErrorMessage(string content)
        {
            return SerializeMessage(new ChatMessage
            {
                Type = MessageType.Error,
                Sender = "SYSTEM",
                Content = content
            });
        }
    }
}
