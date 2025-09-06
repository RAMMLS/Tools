using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using HtmlAgilityPack;

namespace SimpleGoogleBot
{
    public class WebCrawler
    {
        private readonly HttpClient _httpClient;

        public WebCrawler()
        {
            _httpClient = new HttpClient();
            _httpClient.DefaultRequestHeaders.Add("User-Agent", "SimpleGoogleBot/1.0 (C#)"); // Важно!
        }

        public async Task<List<string>> Crawl(string url)
        {
            try
            {
                var response = await _httpClient.GetAsync(url);
                response.EnsureSuccessStatusCode(); // Выбрасывает исключение при ошибке

                string html = await response.Content.ReadAsStringAsync();

                var links = ExtractLinks(html);
                return links;
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"Ошибка при запросе {url}: {ex.Message}");
                return new List<string>(); // Возвращаем пустой список, чтобы не останавливать сканирование
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Неизвестная ошибка при сканировании {url}: {ex.Message}");
                return new List<string>();
            }
        }

        private List<string> ExtractLinks(string html)
        {
            var links = new List<string>();
            var doc = new HtmlAgilityPack.HtmlDocument();
            doc.LoadHtml(html);

            foreach (var link in doc.DocumentNode.SelectNodes("//a[@href]"))
            {
                string hrefValue = link.GetAttributeValue("href", string.Empty);
                if (!string.IsNullOrEmpty(hrefValue) && hrefValue.StartsWith("http")) //Проверяем, что ссылка абсолютная
                {
                    links.Add(hrefValue);
                }
            }

            return links;
        }
    }
}

