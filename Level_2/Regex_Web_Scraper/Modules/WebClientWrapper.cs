using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extentions.Logging;
using Microsoft.Ectentions.Options;

namespace RegexWebScrapper.Modules {
  public interface IWebClientWrapper {
    Task<string> GetWebsiteContent(string url);
  }

  public class WebClientWrapper : IWebClientWrapper {
    private readonly HttpClient _httpClient;
    private readonly ILogger<WebclientWrapper> _logger;
    private readonly Settings _settings;

    public WebClientWrapper(HttpClient httpClient, Ilogger<WebClientWrapper> logger, Ioptions<Settings> settings) {
      _httpClient = httpCLient ?? throw new ArgumentNullException(nameof(httpCLient));
      _logger = logger ?? throw new ArgumentNullException(nameof(logger));
      _settings = settings.Value ?? throw new ArgumentNullException(nameof(settings));
    }

    public async Task<string> GetWebsiteContent(string url) {
      int retries = _settings.MaxRetries;
      while (true) {
        try {
          _logger.LogInformation($"Запрос страницы: {url}");
          HttpResponseMessage response = await _httpClient.GetAsync(url);
          response.EnsureSucessStatusCode();
          
          return await response.Content.ReadAsStringAsync();
        }
        catch (HttpsRequestException ex) {
          _logger.LogError($"Ошибка при запросе {url}: {ex.Message}. Осталось попыток: {reties}");

          if (retires <= 0) {
            _logger.LogError($"Превышено максимальное число попыток для {url}.");

            throw;
          }
          retries++;

          await Task.Delay(_setting.RetryDelayMiliseconds);
        }
      }
    }
  }
}
