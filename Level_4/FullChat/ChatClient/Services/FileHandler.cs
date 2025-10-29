namespace ChatClient.Services
{
    public class FileHandler
    {
        private readonly string _downloadsDirectory;

        public FileHandler()
        {
            _downloadsDirectory = Path.Combine(Directory.GetCurrentDirectory(), "Downloads");
            if (!Directory.Exists(_downloadsDirectory))
                Directory.CreateDirectory(_downloadsDirectory);
        }

        public bool FileExists(string filePath)
        {
            return File.Exists(filePath);
        }

        public long GetFileSize(string filePath)
        {
            return new FileInfo(filePath).Length;
        }

        public byte[] ReadFileChunk(string filePath, long offset, int chunkSize)
        {
            using var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
            stream.Seek(offset, SeekOrigin.Begin);
            
            var buffer = new byte[chunkSize];
            var bytesRead = stream.Read(buffer, 0, chunkSize);
            
            if (bytesRead < chunkSize)
            {
                var trimmedBuffer = new byte[bytesRead];
                Array.Copy(buffer, trimmedBuffer, bytesRead);
                return trimmedBuffer;
            }

            return buffer;
        }

        public void SaveFile(string fileName, byte[] data)
        {
            var filePath = Path.Combine(_downloadsDirectory, fileName);
            
            // Ensure unique filename
            var counter = 1;
            var name = Path.GetFileNameWithoutExtension(fileName);
            var extension = Path.GetExtension(fileName);
            
            while (File.Exists(filePath))
            {
                filePath = Path.Combine(_downloadsDirectory, $"{name} ({counter}){extension}");
                counter++;
            }

            File.WriteAllBytes(filePath, data);
        }

        public string GetDownloadsDirectory()
        {
            return _downloadsDirectory;
        }
    }
}
