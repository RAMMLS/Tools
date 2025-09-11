using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Logging;
using RegexWebScraper.Models;

namespace RegexWebScraper.Modules
{
    public interface IDataExtractor
    {
        List<ScrapedItem> ExtractData(string htmlContent, string dataPattern, string titlePattern);
    }

    public class DataExtractor : IDataExtractor
    {
        private readonly ILogger<DataExtractor> _logger;

        public DataExtractor(ILogger<DataExtractor> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public List<ScrapedItem> ExtractData(string htmlContent, string dataPattern, string titlePattern)
        {
            List<ScrapedItem> scrapedItems = new List<ScrapedItem>();

            try
            {
                // Extract Data
                MatchCollection dataMatches = Regex.Matches(htmlContent, dataPattern, RegexOptions.Singleline);
                // Extract Title
                MatchCollection titleMatches = Regex.Matches(htmlContent, titlePattern, RegexOptions.Singleline);

                for (int i = 0; i < Math.Min(dataMatches.Count, titleMatches.Count); i++)
                {
                    string data = dataMatches[i].Groups[1].Value.Trim();
                    string title = titleMatches[i].Groups[1].Value.Trim();

                    scrapedItems.Add(new ScrapedItem { Title = title, Data = data });
                    _logger.LogInformation($"Извлечено: Title = {title}, Data = {data}");
                }

                return scrapedItems;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Ошибка при извлечении данных: {ex.Message}");
                return new List<ScrapedItem>();
            }
        }
    }
}

