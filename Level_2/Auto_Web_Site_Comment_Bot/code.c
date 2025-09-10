sharp
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Auto_Web_Site_Comment_Bot
{
    class Program
    {
        static async Task Main(string[] args)
        {
            // 1. Загрузка конфигурации
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("Config/settings.json", optional: false, reloadOnChange: true)
                .Build();

            // 2. Настройка логирования (пример с консольным логгером)
            using var loggerFactory = LoggerFactory.Create(builder =>
            {
                builder.AddConfiguration(configuration.GetSection("Logging"))
                       .AddConsole();  //  Логирование в консоль
            });

            ILogger logger = loggerFactory.CreateLogger<Program>();

            // 3. Настройка сервисов (Dependency Injection)
            var serviceProvider = new ServiceCollection()
                .AddSingleton<IConfiguration>(configuration)
                .AddSingleton(loggerFactory) //  Регистрируем фабрику логгеров
                .AddSingleton(typeof(ILogger<>), typeof(Logger<>)) //  Регистрируем обобщенный ILogger
                .AddHttpClient() // Add HttpClient
                .AddTransient<CommentPoster>()
                .AddTransient<WebsiteParser>()
                .AddSingleton<ProxyManager>(sp => new ProxyManager(Path.Combine("Config", "proxies.txt"), sp.GetRequiredService<ILogger<ProxyManager>>()))  // ProxyManager как Singleton
                .AddSingleton<ICaptchaSolver>(sp =>
                {
                    if (configuration.GetValue<bool>("SolveCaptcha"))
                    {
                        string captchaService = configuration.GetValue<string>("CaptchaService");
                        string captchaApiKey = configuration.GetValue<string>("CaptchaApiKey");
                        return CaptchaSolverFactory.CreateSolver(captchaService, captchaApiKey, sp.GetRequiredService<ILogger<CaptchaSolverFactory>>());
                    }
                    return null; //  Если капча не нужна
                })
                .BuildServiceProvider();



            // 4. Получение настроек из конфигурации
            int threads = configuration.GetValue<int>("Threads");
            int commentDelayMin = configuration.GetValue<int>("CommentDelayMin");
            int commentDelayMax = configuration.GetValue<int>("CommentDelayMax");
            int maxCommentAttempts = configuration.GetValue<int>("MaxCommentAttempts");
            bool enableProxy = configuration.GetValue<bool>("EnableProxy");
            bool solveCaptcha = configuration.GetValue<bool>("SolveCaptcha");


            // 5. Загрузка данных (сайты, комментарии)
            List<string> websites = LoadWebsitesFromFile(Path.Combine("Data", "websites.txt"), logger);
            List<string> comments = LoadCommentsFromFile(Path.Combine("Config", "comments.txt"), logger);
            HashSet<string> commentedSites = LoadCommentedSitesFromFile(Path.Combine("Data", "commented_sites.txt"), logger);



            // 6. Основной цикл программы
            List<Task> tasks = new List<Task>();
            for (int i = 0; i < threads; i++)
            {
                tasks.Add(Task.Run(async () =>
                {
                    //  Получаем экземпляры классов из DI контейнера внутри каждого потока
                    var commentPoster = serviceProvider.GetRequiredService<CommentPoster>();
                    var websiteParser = serviceProvider.GetRequiredService<WebsiteParser>();
                    var proxyManager = serviceProvider.GetService<ProxyManager>(); // GetService возвращает null, если не зарегистрирован
                    var captchaSolver = serviceProvider.GetService<ICaptchaSolver>();

                    using (var httpClient = serviceProvider.GetRequiredService<IHttpClientFactory>().CreateClient())
                    {
                        // Configure HttpClient based on settings
                        if (enableProxy && proxyManager != null)
                        {
                            var proxy = proxyManager.GetNextProxy();
                            if (proxy != null)
                            {
                                httpClient.DefaultProxy = proxy;
                                logger.LogInformation($"Поток {Task.CurrentId}: Используется прокси {proxy.Address}");
                            }
                            else
                            {
                                logger.LogWarning($"Поток {Task.CurrentId}: Прокси закончились или не настроены. Работа без прокси.");
                            }
                        }

                        // Create instances for each thread, passing the httpClient and logger
                        var commentPosterThread = new CommentPoster(httpClient, loggerFactory.CreateLogger<CommentPoster>());
                        var websiteParserThread = new WebsiteParser(httpClient, loggerFactory.CreateLogger<WebsiteParser>());

                        while (true) // Бесконечный цикл для работы потока
                        {
                            //  Выбор случайного сайта
                            string websiteUrl = GetRandomWebsite(websites, commentedSites);
                            if (string.IsNullOrEmpty(websiteUrl))
                            {
                                logger.LogInformation($"Поток {Task.CurrentId}: Все сайты обработаны. Завершение работы.");
                                break; //  Выход из цикла, если все сайты обработаны
                            }

                            // Выбор случайного комментария
                            string comment = GetRandomComment(comments);

                            // Парсинг сайта для получения структуры формы
                            var formDetails = await websiteParserThread.ParseCommentForm(websiteUrl);
                            if (formDetails == null)
                            {
                                logger.LogError($"Поток {Task.CurrentId}: Не удалось получить структуру формы для сайта {websiteUrl}");
                                continue; // Переходим к следующему сайту
                            }

                            //  Заполнение данных комментария (пример)
                            string authorName = "John Doe";
                            string authorEmail = "john.doe@example.com";

                            //  Отправка комментария
                            bool commentSent = false;
                            for (int attempt = 1; attempt <= maxCommentAttempts; attempt++)
                            {
                                logger.LogInformation($"Поток {Task.CurrentId}: Попытка {attempt} отправки комментария на сайт {websiteUrl}");

                                commentSent = await commentPosterThread.PostComment(websiteUrl, comment, authorName, authorEmail);

                                if (commentSent)
                                {
                                    break;
                                }
                                else
                                {
                                    logger.LogWarning($"Поток {Task.CurrentId}: Попытка {attempt} отправки комментария на сайт {websiteUrl} не удалась.");
                                    await Task.Delay(TimeSpan.FromSeconds(5)); // Пауза перед следующей попыткой
                                }
                            }


                            if (commentSent)
                            {
                                //  Добавление сайта в список прокомментированных
                                lock (commentedSites)
                                {
                                    commentedSites.Add(websiteUrl);
                                    SaveCommentedSitesToFile(Path.Combine("Data", "commented_sites.txt"), commentedSites, logger);
                                }
                            }
                            else
                            {
                                logger.LogError($"Поток {Task.CurrentId}: Не удалось отправить комментарий на сайт {websiteUrl} после {maxCommentAttempts} попыток.");
                            }

                            // Случайная задержка перед следующим комментарием
                            int delay = new Random().Next(commentDelayMin, commentDelayMax + 1);
                            logger.LogInformation($"Поток {Task.CurrentId}: Задержка перед следующим комментарием: {delay} секунд.");
                            await Task.Delay(TimeSpan.FromSeconds(delay));
                        }
                    }
                }));
            }

            await Task.WhenAll(tasks); // Ожидание завершения всех потоков

            Console.WriteLine("Работа программы завершена.");
        }


        // Методы для загрузки и сохранения данных (сайты, комментарии, прокомментированные сайты)
        static List<string> LoadWebsitesFromFile(string filePath, ILogger logger)
        {
            List<string> websites = new List<string>();
            try
            {
                if (File.Exists(filePath))
                {
                    websites = new List<string>(File.ReadAllLines(filePath));
                    logger.LogInformation($"Загружено {websites.Count} сайтов из файла {filePath}");
                }
                else
                {
                    logger.LogWarning($"Файл с сайтами не найден: {filePath}");
                }
            }
            catch (Exception ex)
            {
                logger.LogError($"Ошибка при загрузке сайтов из файла {filePath}: {ex.Message}");
            }
            return websites;
        }

        static List<string> LoadCommentsFromFile(string filePath, ILogger logger)
        {
            List<string> comments = new List<string>();
            try
            {
                if (File.Exists(filePath))
                {
                    comments = new List<string>(File.ReadAllLines(filePath));
                    logger.LogInformation($"Загружено {comments.Count} комментариев из файла {filePath}");
                }
                else
                {
                    logger.LogWarning($"Файл с комментариями не найден: {filePath}");
                }
            }
            catch (Exception ex)
            {
                logger.LogError($"Ошибка при загрузке комментариев из файла {filePath}: {ex.Message}");
            }
            return comments;
        }


        static HashSet<string> LoadCommentedSitesFromFile(string filePath, ILogger logger)
        {
            HashSet<string> commentedSites = new HashSet<string>();
            try
            {
                if (File.Exists(filePath))
                {
                    string[] lines = File.ReadAllLines(filePath);
                    foreach (string line in lines)
                    {
                        commentedSites.Add(line.Trim());
                    }
                    logger.LogInformation($"Загружено {commentedSites.Count} прокомментированных сайтов из файла {filePath}");
                }
                else
                {
                    logger.LogWarning($"Файл с прокомментированными сайтами не найден: {filePath}. Будет создан новый файл.");
                }
            }
            catch (Exception ex)
            {
                logger.LogError($"Ошибка при загрузке прокомментированных сайтов из файла {filePath}: {ex.Message}");
            }
            return commentedSites;
        }

        static void SaveCommentedSitesToFile(string filePath, HashSet<string> commentedSites, ILogger logger)
        {
            try
            {
                File.WriteAllLines(filePath, commentedSites);
                logger.LogInformation($"Сохранено {commentedSites.Count} прокомментированных сайтов в файл {filePath}");
            }
            catch (Exception ex)
            {
                logger.LogError($"Ошибка при сохранении прокомментированных сайтов в файл {filePath}: {ex.Message}");
            }
        }


        // Методы для получения случайных данных
        static string GetRandomWebsite(List<string> websites, HashSet<string> commentedSites)
        {
            List<string> availableWebsites = new List<string>();
            foreach (string website in websites)
            {
                if (!commentedSites.Contains(website))
                {
                    availableWebsites.Add(website);
                }
            }

            if (availableWebsites.Count == 0)
            {
                return null; // Все сайты уже прокомментированы
            }

            return availableWebsites[new Random().Next(availableWebsites.Count)];
        }


        static string GetRandomComment(List<string> comments)
        {
            return comments[new Random().Next(comments.Count)];
        }

    }
}
