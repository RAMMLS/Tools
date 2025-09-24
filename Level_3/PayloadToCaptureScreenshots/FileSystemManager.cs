using System;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;
using System.Security.Cryptography;

namespace ScreenshotCaptureTool
{
    public class FileSystemManager
    {
        private readonly string baseDirectory;
        private readonly string screenshotsFolder;
        
        public FileSystemManager()
        {
            baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
            screenshotsFolder = Path.Combine(baseDirectory, "Screenshots");
            
            InitializeFileSystem();
        }
        
        private void InitializeFileSystem()
        {
            try
            {
                if (!Directory.Exists(screenshotsFolder))
                {
                    Directory.CreateDirectory(screenshotsFolder);
                    SetFolderAttributes(screenshotsFolder);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"File system init failed: {ex.Message}");
            }
        }
        
        private void SetFolderAttributes(string folderPath)
        {
            try
            {
                // Установка скрытых атрибутов (опционально)
                DirectoryInfo di = new DirectoryInfo(folderPath);
                di.Attributes |= FileAttributes.Hidden;
            }
            catch
            {
                // Игнорируем ошибки атрибутов
            }
        }
        
        public string SaveScreenshot(Bitmap screenshot)
        {
            string fileName = GenerateFileName();
            string filePath = Path.Combine(screenshotsFolder, fileName);
            
            try
            {
                screenshot.Save(filePath, ImageFormat.Jpeg);
                return filePath;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Save failed: {ex.Message}");
                return null;
            }
        }
        
        private string GenerateFileName()
        {
            string timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss_fff");
            string randomPart = GenerateRandomString(6);
            return $"ss_{timestamp}_{randomPart}.jpg";
        }
        
        private string GenerateRandomString(int length)
        {
            using (var rng = new RNGCryptoServiceProvider())
            {
                byte[] tokenData = new byte[length];
                rng.GetBytes(tokenData);
                return Convert.ToBase64String(tokenData)
                    .Replace("+", "")
                    .Replace("/", "")
                    .Replace("=", "")
                    .Substring(0, length);
            }
        }
        
        public void CleanOldFiles(TimeSpan olderThan)
        {
            try
            {
                var files = Directory.GetFiles(screenshotsFolder, "*.jpg");
                foreach (var file in files)
                {
                    var fileInfo = new FileInfo(file);
                    if (DateTime.Now - fileInfo.LastWriteTime > olderThan)
                    {
                        File.Delete(file);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Cleanup failed: {ex.Message}");
            }
        }
    }
}
