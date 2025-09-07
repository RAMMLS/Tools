using System;
using System.Threading.Tasks;

namespace SimpleGoogleBot
{
    class Program
    {
        static async Task Main(string[] args)
        {
            string startUrl = "https://www.google.com";  // Замените на любой другой сайт
            string outputDirectory = "output";

            WebCrawler crawler = new WebCrawler();
            FileStorage storage = new FileStorage(outputDirectory);

            Console.WriteLine($"Начинаем сканирование с {startUrl} и сохраняем в {outputDirectory}");

            try
            {
                var links = await crawler.Crawl(startUrl);

                Console.WriteLine($"Найдено {links.Count} ссылок на странице {startUrl}");

                foreach (var link in links)
                {
                    try
                    {
                        var pageContent = await crawler.Crawl(link); //  Получаем контент страницы
                        storage.SavePage(link, string.Join("\n", pageContent)); // Сохраняем контент
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Ошибка при обработке ссылки {link}: {ex.Message}");
                    }

                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Критическая ошибка: {ex.Message}");
            }

            Console.WriteLine("Сканирование завершено.");
        }
    }
}

