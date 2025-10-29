using System.Net;
using System.Net.Sockets;
using ChatServer.Models;
using ChatServer.Protocols;
using ChatServer.Services;
using ChatShared.Enums;

namespace ChatServer.Services
{
    public class ChatService
    {
        private readonly TcpListener _listener;
        private readonly UserManager _userManager;
        private readonly FileTransferService _fileTransferService;
        private readonly MessageProcessor _messageProcessor;
        private bool _isRunning;

        public ChatService(int port = 5000)
        {
            _listener = new TcpListener(IPAddress.Any, port);
            _userManager = new UserManager();
            _fileTransferService = new FileTransferService();
            _messageProcessor = new MessageProcessor(_userManager, _fileTransferService);

            SetupEventHandlers();
        }

        private void SetupEventHandlers()
        {
            _userManager.UserConnected += OnUserConnected;
            _userManager.UserDisconnected += OnUserDisconnected;
        }

        public async Task StartAsync()
        {
            _listener.Start();
            _isRunning = true;

            Console.WriteLine($"Chat server started on port {((IPEndPoint)_listener.LocalEndpoint).Port}");
            Console.WriteLine("Waiting for connections...");

            while (_isRunning)
            {
                try
                {
                    var client = await _listener.AcceptTcpClientAsync();
                    _ = Task.Run(() => HandleClientAsync(client));
                }
                catch (ObjectDisposedException)
                {
                    break;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error accepting client: {ex.Message}");
                }
            }
        }

        public void Stop()
        {
            _isRunning = false;
            _listener.Stop();
            
            // Disconnect all users
            var onlineUsers = _userManager.GetOnlineUsers();
            foreach (var user in onlineUsers)
            {
                user.Disconnect();
            }
        }

        private async Task HandleClientAsync(TcpClient client)
        {
            User? user = null;

            try
            {
                user = new User(client);
                Console.WriteLine($"New connection from {client.Client.RemoteEndPoint}");

                // Authentication phase
                var username = await AuthenticateUserAsync(user);
                if (string.IsNullOrEmpty(username))
                {
                    await user.Writer.WriteAsync(MessageProtocol.CreateErrorMessage("Authentication failed"));
                    return;
                }

                if (!_userManager.AddUser(user, username))
                {
                    await user.Writer.WriteAsync(MessageProtocol.CreateErrorMessage("Username already taken"));
                    return;
                }

                await user.Writer.WriteAsync(MessageProtocol.CreateSystemMessage("Authentication successful!"));
                await user.Writer.WriteAsync(MessageProtocol.CreateSystemMessage("Welcome to the chat server!"));
                await user.Writer.WriteAsync(MessageProtocol.CreateSystemMessage("Commands: /list, /private <user> <message>, /file <user> <filepath>"));

                // Main message loop
                await ProcessUserMessagesAsync(user);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error handling client: {ex.Message}");
            }
            finally
            {
                if (user != null)
                {
                    _userManager.RemoveUser(user.Id);
                }
                else
                {
                    client.Close();
                }
            }
        }

        private async Task<string?> AuthenticateUserAsync(User user)
        {
            try
            {
                // Send welcome message
                await user.Writer.WriteAsync(MessageProtocol.CreateSystemMessage("Enter your username:"));

                var usernameLine = await user.Reader.ReadLineAsync();
                if (string.IsNullOrWhiteSpace(usernameLine))
                    return null;

                var username = usernameLine.Trim();
                if (username.Length < 3 || username.Length > 20)
                {
                    await user.Writer.WriteAsync(MessageProtocol.CreateErrorMessage("Username must be 3-20 characters"));
                    return null;
                }

                if (_userManager.IsUsernameTaken(username))
                {
                    await user.Writer.WriteAsync(MessageProtocol.CreateErrorMessage("Username already taken"));
                    return null;
                }

                return username;
            }
            catch
            {
                return null;
            }
        }

        private async Task ProcessUserMessagesAsync(User user)
        {
            try
            {
                while (user.IsOnline)
                {
                    var messageJson = await user.Reader.ReadLineAsync();
                    if (string.IsNullOrEmpty(messageJson))
                        break;

                    var message = MessageProtocol.DeserializeMessage(messageJson);
                    if (message != null)
                    {
                        message.Sender = user.Username;
                        await _messageProcessor.ProcessMessageAsync(message, user);
                    }
                }
            }
            catch
            {
                // Client disconnected
            }
        }

        private async void OnUserConnected(User user)
        {
            Console.WriteLine($"User '{user.Username}' connected");

            // Notify all users
            var joinMessage = MessageProtocol.SerializeMessage(new ChatMessage
            {
                Type = MessageType.UserJoined,
                Sender = user.Username,
                Content = $"{user.Username} joined the chat"
            });

            await BroadcastToAllAsync(joinMessage);
            await SendUserListToAllAsync();
        }

        private async void OnUserDisconnected(User user)
        {
            Console.WriteLine($"User '{user.Username}' disconnected");

            // Notify all users
            var leaveMessage = MessageProtocol.SerializeMessage(new ChatMessage
            {
                Type = MessageType.UserLeft,
                Sender = user.Username,
                Content = $"{user.Username} left the chat"
            });

            await BroadcastToAllAsync(leaveMessage);
            await SendUserListToAllAsync();
        }

        private async Task BroadcastToAllAsync(string message)
        {
            var onlineUsers = _userManager.GetOnlineUsers();
            var tasks = onlineUsers.Select(u => SendToUserAsync(u, message));
            await Task.WhenAll(tasks);
        }

        private async Task SendUserListToAllAsync()
        {
            var userList = _userManager.GetOnlineUsernames();
            var userListMessage = MessageProtocol.CreateUserListMessage(userList);
            await BroadcastToAllAsync(userListMessage);
        }

        private async Task SendToUserAsync(User user, string message)
        {
            try
            {
                if (user.IsOnline)
                {
                    await user.Writer.WriteAsync(message);
                }
            }
            catch
            {
                _userManager.RemoveUser(user.Id);
            }
        }
    }
}
