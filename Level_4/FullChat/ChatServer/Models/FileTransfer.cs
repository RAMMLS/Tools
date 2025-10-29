using ChatShared.Enums;

namespace ChatServer.Models
{
    public class FileTransfer
    {
        public string TransferId { get; set; } = Guid.NewGuid().ToString();
        public string FileName { get; set; } = string.Empty;
        public long FileSize { get; set; }
        public string Sender { get; set; } = string.Empty;
        public string Recipient { get; set; } = string.Empty;
        public FileTransferStatus Status { get; set; }
        public DateTime StartedAt { get; set; } = DateTime.Now;
        public DateTime? CompletedAt { get; set; }
        public string FilePath { get; set; } = string.Empty;
    }
}
