using System;
using System.Net.Http;
using System.Threading.Tasks;

public class CommentPoster
{
    private readonly HttpClient _httpClient;
    private readonly ILogger _logger;

    public CommentPoster(HttpClient httpClient, ILogger logger)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<bool> PostComment(string websiteUrl, string comment, string authorName, string authorEmail)
    {
        try
        {
            // 1. Определить структуру формы для комментария (поля, имена) - это делается в WebsiteParser
            // 2. Сформировать POST запрос с данными комментария
            // 3. Отправить запрос и проверить результат
            _logger.LogInformation($"Попытка отправки комментария на сайт: {websiteUrl}");

            //  Пример отправки POST запроса (нужно адаптировать под конкретный сайт)
            var formContent = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("comment", comment),
                new KeyValuePair<string, string>("author", authorName),
                new KeyValuePair<string, string>("email", authorEmail)
            });

            var response = await _httpClient.PostAsync(websiteUrl, formContent);
            response.EnsureSuccessStatusCode();  // Проверка на HTTP ошибки

            string responseBody = await response.Content.ReadAsStringAsync();

            // 4. Проверить ответ сервера (успешное добавление комментария) - анализировать responseBody
            if (responseBody.Contains("Комментарий успешно добавлен")) // Пример проверки
            {
                _logger.LogInformation($"Комментарий успешно отправлен на сайт: {websiteUrl}");
                return true;
            }
            else
            {
                _logger.LogWarning($"Не удалось отправить комментарий на сайт: {websiteUrl}. Ответ сервера: {responseBody}");
                return false;
            }
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError($"Ошибка при отправке запроса на сайт {websiteUrl}: {ex.Message}");
            return false;
        }
        catch (Exception ex)
        {
            _logger.LogError($"Непредвиденная ошибка при отправке комментария на сайт {websiteUrl}: {ex.Message}");
            return false;
        }
    }
}

