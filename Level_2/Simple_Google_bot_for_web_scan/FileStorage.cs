using System;
using System.IO;
using System.Text;

namespace SimpleGoogleBot
{
    public class FileStorage
    {
        private readonly string _rootDirectory;
        private DirectoryNode _rootNode;

        public FileStorage(string rootDirectory)
        {
            _rootDirectory = rootDirectory;
            if (!Directory.Exists(_rootDirectory))
            {
                Directory.CreateDirectory(_rootDirectory);
            }

            _rootNode = new DirectoryNode(rootDirectory, null);
        }

        public void SavePage(string url, string content)
        {
            // Используем URL для создания пути к файлу
            string filePath = GenerateFilePath(url);

            try
            {
                 string directoryPath = Path.GetDirectoryName(filePath);
                if (!Directory.Exists(directoryPath))
                {
                    Directory.CreateDirectory(directoryPath);

                     DirectoryNode currentDir = FindOrCreateDirectoryNode(directoryPath);
                }

                File.WriteAllText(filePath, content, Encoding.UTF8);

                Console.WriteLine($"Страница {url} сохранена в {filePath}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при сохранении страницы {url}: {ex.Message}");
            }
        }

        private string GenerateFilePath(string url)
        {
            // Заменяем недопустимые символы в URL на безопасные
            string safeUrl = string.Join("_", url.Split(Path.GetInvalidFileNameChars()));

            // Ограничиваем длину имени файла
            if (safeUrl.Length > 200)
            {
                safeUrl = safeUrl.Substring(0, 200);
            }
              string filePath = Path.Combine(_rootDirectory, safeUrl + ".html");
              return filePath;

        }

        //Рекурсивно ищем или создаем DirectoryNode
          private DirectoryNode FindOrCreateDirectoryNode(string path)
        {
              DirectoryNode currentDir = _rootNode;
            string[] parts = path.Substring(_rootDirectory.Length).Split(Path.DirectorySeparatorChar, StringSplitOptions.RemoveEmptyEntries);

                foreach (string part in parts)
                {
                     DirectoryNode nextDir = currentDir.Children.Find(n => n.Name == part) as DirectoryNode;
                      if (nextDir == null)
                    {
                        string fullPath = Path.Combine(currentDir.FullPath, part);
                          nextDir = new DirectoryNode(part, currentDir);
                           Directory.CreateDirectory(fullPath); // Create the physical directory
                           currentDir.Children.Add(nextDir);
                    }
                       currentDir = nextDir;

                }

            return currentDir;
        }
    }
}

