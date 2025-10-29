using ChatServer.Models;
using ChatServer.Protocols;
using ChatServer.Services;
using ChatShared.Enums;

namespace ChatServer.Services
{
    public class MessageProcessor
    {
        private readonly UserManager _userManager;
        private readonly FileTransferService _fileTransferService;

        public MessageProcessor(UserManager userManager, FileTransferService fileTransferService)
        {
            _userManager = userManager;
            _fileTransferService = fileTransferService;
        }

        public async Task ProcessMessageAsync(ChatMessage message, User sender)
        {
            try
            {
                switch (message.Type)
                {
                    case MessageType.Text:
                        await BroadcastMessageAsync(message);
                        break;

                    case MessageType.Private:
                        await SendPrivateMessageAsync(message);
                        break;

                    case MessageType.File:
                        await HandleFileTransferAsync(message, sender);
                        break;

                    default:
                        await SendToUserAsync(sender, MessageProtocol.CreateErrorMessage("Unknown message type"));
                        break;
                }
            }
            catch (Exception ex)
            {
                await SendToUserAsync(sender, MessageProtocol.CreateErrorMessage($"Error: {ex.Message}"));
            }
        }

        private async Task BroadcastMessageAsync(ChatMessage message)
        {
            var onlineUsers = _userManager.GetOnlineUsers();
            var tasks = onlineUsers.Select(user => SendToUserAsync(user, MessageProtocol.SerializeMessage(message)));
            await Task.WhenAll(tasks);
        }

        private async Task SendPrivateMessageAsync(ChatMessage message)
        {
            var recipient = _userManager.GetUserByUsername(message.Recipient);
            if (recipient != null && recipient.IsOnline)
            {
                await SendToUserAsync(recipient, MessageProtocol.SerializeMessage(message));
                // Also send confirmation to sender
                var sender = _userManager.GetUserByUsername(message.Sender);
                if (sender != null)
                {
                    await SendToUserAsync(sender, MessageProtocol.SerializeMessage(message));
                }
            }
            else
            {
                var sender = _userManager.GetUserByUsername(message.Sender);
                if (sender != null)
                {
                    await SendToUserAsync(sender, 
                        MessageProtocol.CreateErrorMessage($"User '{message.Recipient}' is not online"));
                }
            }
        }

        private async Task HandleFileTransferAsync(ChatMessage message, User sender)
        {
            if (message.Metadata.TryGetValue("transferId", out var transferId) &&
                message.Metadata.TryGetValue("fileName", out var fileName) &&
                message.Metadata.TryGetValue("fileSize", out var fileSizeStr) &&
                long.TryParse(fileSizeStr, out var fileSize))
            {
                var transfer = _fileTransferService.StartFileTransfer(fileName, fileSize, sender.Username, message.Recipient);
                if (transfer != null)
                {
                    // Notify recipient about file transfer request
                    var recipient = _userManager.GetUserByUsername(message.Recipient);
                    if (recipient != null && recipient.IsOnline)
                    {
                        var fileRequestMessage = new ChatMessage
                        {
                            Type = MessageType.File,
                            Sender = sender.Username,
                            Recipient = message.Recipient,
                            Content = $"File transfer request: {fileName} ({FormatFileSize(fileSize)})",
                            Metadata = new Dictionary<string, string>
                            {
                                ["transferId"] = transfer.TransferId,
                                ["fileName"] = fileName,
                                ["fileSize"] = fileSize.ToString(),
                                ["action"] = "request"
                            }
                        };

                        await SendToUserAsync(recipient, MessageProtocol.SerializeMessage(fileRequestMessage));
                        await SendToUserAsync(sender, MessageProtocol.CreateSystemMessage($"File transfer request sent to {message.Recipient}"));
                    }
                    else
                    {
                        await SendToUserAsync(sender, MessageProtocol.CreateErrorMessage($"User '{message.Recipient}' is not online"));
                        _fileTransferService.FailFileTransfer(transfer.TransferId);
                    }
                }
            }
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
                // User disconnected
                _userManager.RemoveUser(user.Id);
            }
        }

        private string FormatFileSize(long bytes)
        {
            string[] sizes = { "B", "KB", "MB", "GB" };
            int order = 0;
            double len = bytes;
            while (len >= 1024 && order < sizes.Length - 1)
            {
                order++;
                len /= 1024;
            }
            return $"{len:0.##} {sizes[order]}";
        }
    }
}
