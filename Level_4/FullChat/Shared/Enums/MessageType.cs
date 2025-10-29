namespace ChatShared.Enums
{
    public enum MessageType
    {
        Text,
        Private,
        File,
        System,
        UserJoined,
        UserLeft,
        UserList,
        Error
    }

    public enum FileTransferStatus
    {
        Pending,
        InProgress,
        Completed,
        Failed,
        Cancelled
    }
}
