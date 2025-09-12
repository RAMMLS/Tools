using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RegexWebScraper.Modules;
using RegexWebScraper.Models;

namespace RegexWebScraper
{
    public class Program
    {
        public class Settings
        {
            public string WebsiteUrl { get; set; }
            public string DataPattern { get; set; }
            public string TitlePattern { get; set; }
            public bool EnableLogging { get; set; }
            public int MaxRetries { get; set; }
            public int RetryDelayMilliseconds { get; set; }
        }

        static async Task Main(string[] args)
        {
            // 1. Настройка конфигурации
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("Config/settings.json", optional: false, reloadOnChange: true)
                .Build();

            // 2. Настройка логирования
            using var loggerFactory = LoggerFactory.Create(builder =>
            {
                builder.AddConfiguration(configuration.GetSection("Logging"))
                       .AddConsole();
            });

            ILogger<Program> logger = loggerFactory.CreateLogger<Program>();

            // 3. Настройка сервисов (Dependency Injection)
            var serviceProvider = new ServiceCollection()
                .AddSingleton<IConfiguration>(configuration)
                .AddSingleton(loggerFactory)
                .AddSingleton(typeof(ILogger<>), typeof(Logger<>))
                .AddHttpClient()
                .Configure<Settings>(configuration)
                .AddTransient<IWebClientWrapper, WebClientWrapper>()
                .AddTransient<IDataExtractor, DataExtractor>()
                .AddTransient<IDataWriter, DataWriter>()
                .BuildServiceProvider();

            // 4. Получение настроек из DI контейнера
            var settings = serviceProvider.GetRequiredService<IOptions<Settings>>().Value;
            var webClient = serviceProvider.GetRequiredService<IWebClientWrapper>();
            var dataExtractor = serviceProvider.GetRequiredService<IDataExtractor>();
            var dataWriter = serviceProvider.GetRequiredService<IDataWriter>();

            try
            {
                // 5. Загрузка HTML контента веб-сайта
                string htmlContent = await webClient.GetWebsiteContent(settings.WebsiteUrl);

                // 6. Извлечение данных с помощью Regex
                List<ScrapedItem> scrapedData = dataExtractor.ExtractData(htmlContent, settings.DataPattern, settings.TitlePattern);

                // 7. Запись данных в файл
                dataWriter.WriteDataToFile(scrapedData, Path.Combine("Data", "scraped_data.txt"));

                logger.LogInformation("Web scraping завершен успешно.");
            }
            catch (Exception ex)
            {
                logger.LogError($"Произошла ошибка: {ex.Message}");
            }
        }
    }
}

