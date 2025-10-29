using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using ChatClient.Models;
using ChatShared.Enums;

namespace ChatClient.Services
{
    public class NetworkClient
    {
        private TcpClient? _client;
        private StreamReader? _reader;
        private StreamWriter? _writer;
        private bool _isConnected;
        private readonly FileHandler _fileHandler;

        public event Action<Message>? MessageReceived;
        public event Action<string>? ConnectionLost;

        public NetworkClient()
        {
            _fileHandler = new FileHandler();
        }

        public async Task<bool> ConnectAsync(string server, int port)
        {
            try
            {
                _client = new TcpClient();
                await _client.ConnectAsync(server, port);
                
                var stream = _client.GetStream();
                _reader = new StreamReader(stream);
                _writer = new StreamWriter(stream) { AutoFlush = true };
                
                _isConnected = true;
                
                // Start listening for messages
                _ = Task.Run(ListenForMessagesAsync);
                
                return true;
            }
            catch (Exception ex)
            {
                ConnectionLost?.Invoke($"Connection failed: {ex.Message}");
                return false;
            }
        }

        public void Disconnect()
        {
            _isConnected = false;
            _reader?.Close();
            _writer?.Close();
            _client?.Close();
        }

        public async Task SendMessageAsync(string content, string recipient = "")
        {
            if (!_isConnected || _writer == null) return;

            try
            {
                var messageType = string.IsNullOrEmpty(recipient) ? MessageType.Text : MessageType.Private;
                
                var messageData = new
                {
                    type = messageType.ToString(),
                    sender = string.Empty, // Will be filled by server
                    recipient = recipient,
                    content = content,
                    timestamp = DateTime.Now
                };

                var json = JsonSerializer.Serialize(messageData) + "\n";
                await _writer.WriteAsync(json);
            }
            catch
            {
                ConnectionLost?.Invoke("Failed to send message");
            }
        }

        public async Task SendFileAsync(string filePath, string recipient)
        {
            if (!_isConnected || _writer == null) return;

            try
            {
                if (!_fileHandler.FileExists(filePath))
                {
                    MessageReceived?.Invoke(new Message
                    {
                        Type = MessageType.Error,
                        Content = $"File not found: {filePath}"
                    });
                    return;
                }

                var fileName = Path.GetFileName(filePath);
                var fileSize = _fileHandler.GetFileSize(filePath);

                var messageData = new
                {
                    type = MessageType.File.ToString(),
                    sender = string.Empty,
                    recipient = recipient,
                    content = $"File: {fileName}",
                    timestamp = DateTime.Now,
                    metadata = new Dictionary<string, string>
                    {
                        ["fileName"] = fileName,
                        ["fileSize"] = fileSize.ToString(),
                        ["action"] = "request"
                    }
                };

                var json = JsonSerializer.Serialize(messageData) + "\n";
                await _writer.WriteAsync(json);

                MessageReceived?.Invoke(new Message
                {
                    Type = MessageType.System,
                    Content = $"File transfer request sent for: {fileName}"
                });
            }
            catch (Exception ex)
            {
                MessageReceived?.Invoke(new Message
                {
                    Type = MessageType.Error,
                    Content = $"Failed to send file: {ex.Message}"
                });
            }
        }

        public async Task AcceptFileTransferAsync(string transferId, string fileName, long fileSize)
        {
            if (!_isConnected || _writer == null) return;

            try
            {
                var messageData = new
                {
                    type = MessageType.File.ToString(),
                    sender = string.Empty,
                    recipient = string.Empty,
                    content = "File transfer accepted",
                    timestamp = DateTime.Now,
                    metadata = new Dictionary<string, string>
                    {
                        ["transferId"] = transferId,
                        ["action"] = "accept"
                    }
                };

                var json = JsonSerializer.Serialize(messageData) + "\n";
                await _writer.WriteAsync(json);

                MessageReceived?.Invoke(new Message
                {
                    Type = MessageType.System,
                    Content = $"Accepted file transfer: {fileName}"
                });
            }
            catch (Exception ex)
            {
                MessageReceived?.Invoke(new Message
                {
                    Type = MessageType.Error,
                    Content = $"Failed to accept file transfer: {ex.Message}"
                });
            }
        }

        private async Task ListenForMessagesAsync()
        {
            try
            {
                while (_isConnected && _reader != null)
                {
                    var messageJson = await _reader.ReadLineAsync();
                    if (string.IsNullOrEmpty(messageJson))
                        break;

                    ProcessIncomingMessage(messageJson);
                }
            }
            catch
            {
                // Connection lost
            }
            finally
            {
                _isConnected = false;
                ConnectionLost?.Invoke("Connection to server lost");
            }
        }

        private void ProcessIncomingMessage(string messageJson)
        {
            try
            {
                var messageData = JsonSerializer.Deserialize<JsonElement>(messageJson);
                
                var message = new Message
                {
                    Type = Enum.Parse<MessageType>(messageData.GetProperty("type").GetString() ?? "Text"),
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

                MessageReceived?.Invoke(message);
            }
            catch (Exception ex)
            {
                MessageReceived?.Invoke(new Message
                {
                    Type = MessageType.Error,
                    Content = $"Failed to parse message: {ex.Message}"
                });
            }
        }

        public bool IsConnected => _isConnected;
    }
}
