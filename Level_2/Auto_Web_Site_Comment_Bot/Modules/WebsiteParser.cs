using System;
using System.Net.Http;
using System.Threading.Tasks;
using HtmlAgilityPack;

public class WebsiteParser
{
    private readonly HttpClient _httpClient;
     private readonly ILogger _logger;

    public WebsiteParser(HttpClient httpClient, ILogger logger)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
         _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<CommentFormDetails> ParseCommentForm(string websiteUrl)
    {
        try
        {
            var response = await _httpClient.GetAsync(websiteUrl);
            response.EnsureSuccessStatusCode();
            string htmlContent = await response.Content.ReadAsStringAsync();

            HtmlDocument htmlDocument = new HtmlDocument();
            htmlDocument.LoadHtml(htmlContent);

            //  Здесь нужно реализовать логику парсинга HTML для поиска формы комментария
            //  Например, поиск по id, name, class атрибутам формы и её полей

            //  Пример:
            var commentForm = htmlDocument.DocumentNode.SelectSingleNode("//form[@id='commentform']");

            if (commentForm == null)
            {
                _logger.LogWarning($"Форма комментария не найдена на сайте: {websiteUrl}");
                return null; // Или выбросить исключение
            }

            // Извлекаем имена полей (input, textarea) и другие атрибуты
            string commentFieldName = GetInputName(commentForm, "comment");
            string authorFieldName = GetInputName(commentForm, "author");
            string emailFieldName = GetInputName(commentForm, "email");

            if (string.IsNullOrEmpty(commentFieldName) || string.IsNullOrEmpty(authorFieldName) || string.IsNullOrEmpty(emailFieldName))
            {
                 _logger.LogWarning($"Не удалось определить имена всех полей формы комментария на сайте: {websiteUrl}");
                return null;
            }


            return new CommentFormDetails
            {
                CommentFieldName = commentFieldName,
                AuthorFieldName = authorFieldName,
                EmailFieldName = emailFieldName
            };
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError($"Ошибка при загрузке страницы {websiteUrl}: {ex.Message}");
            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError($"Непредвиденная ошибка при парсинге сайта {websiteUrl}: {ex.Message}");
            return null;
        }
    }

    private string GetInputName(HtmlNode formNode, string fieldId)
    {
         //  Реализовать логику поиска имени поля по id, name или другим атрибутам
         //  Например, поиск по `input[@id='comment']/@name`

         var node = formNode.SelectSingleNode($".//input[@id='{fieldId}']/@name") ?? formNode.SelectSingleNode($".//textarea[@id='{fieldId}']/@name");
         return node?.Value;
    }
}

public class CommentFormDetails
{
    public string CommentFieldName { get; set; }
    public string AuthorFieldName { get; set; }
    public string EmailFieldName { get; set; }
    // Другие поля, например, поле для URL, CAPTCHA
}

