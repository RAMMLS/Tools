using System.Collections.Concurrent;
using ChatServer.Models;
using ChatShared.Enums;

namespace ChatServer.Services
{
    public class FileTransferService
    {
        private readonly ConcurrentDictionary<string, FileTransfer> _transfers = new();
        private readonly string _filesDirectory;

        public FileTransferService()
        {
            _filesDirectory = Path.Combine(Directory.GetCurrentDirectory(), "Files");
            if (!Directory.Exists(_filesDirectory))
                Directory.CreateDirectory(_filesDirectory);
        }

        public FileTransfer? StartFileTransfer(string fileName, long fileSize, string sender, string recipient)
        {
            var transfer = new FileTransfer
            {
                FileName = fileName,
                FileSize = fileSize,
                Sender = sender,
                Recipient = recipient,
                Status = FileTransferStatus.Pending,
                FilePath = Path.Combine(_filesDirectory, $"{Guid.NewGuid()}_{fileName}")
            };

            return _transfers.TryAdd(transfer.TransferId, transfer) ? transfer : null;
        }

        public bool CompleteFileTransfer(string transferId)
        {
            if (_transfers.TryGetValue(transferId, out var transfer))
            {
                transfer.Status = FileTransferStatus.Completed;
                transfer.CompletedAt = DateTime.Now;
                return true;
            }
            return false;
        }

        public bool FailFileTransfer(string transferId)
        {
            if (_transfers.TryGetValue(transferId, out var transfer))
            {
                transfer.Status = FileTransferStatus.Failed;
                
                // Clean up file if it exists
                if (File.Exists(transfer.FilePath))
                    File.Delete(transfer.FilePath);

                return true;
            }
            return false;
        }

        public FileTransfer? GetTransfer(string transferId)
        {
            return _transfers.TryGetValue(transferId, out var transfer) ? transfer : null;
        }

        public void SaveFileChunk(string transferId, byte[] data)
        {
            if (_transfers.TryGetValue(transferId, out var transfer))
            {
                using var stream = new FileStream(transfer.FilePath, FileMode.Append, FileAccess.Write);
                stream.Write(data, 0, data.Length);
            }
        }

        public byte[]? GetFileData(string transferId)
        {
            if (_transfers.TryGetValue(transferId, out var transfer) && File.Exists(transfer.FilePath))
            {
                return File.ReadAllBytes(transfer.FilePath);
            }
            return null;
        }

        public void CleanupCompletedTransfers()
        {
            var completedTransfers = _transfers.Values
                .Where(t => t.Status == FileTransferStatus.Completed && 
                           t.CompletedAt < DateTime.Now.AddHours(-1))
                .ToList();

            foreach (var transfer in completedTransfers)
            {
                if (_transfers.TryRemove(transfer.TransferId, out _) && File.Exists(transfer.FilePath))
                {
                    File.Delete(transfer.FilePath);
                }
            }
        }
    }
}
