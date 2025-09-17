using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using WordPressSecurityTrainingTool.Utilities;

namespace WordPressSecurityTrainingTool.Core
{
    public class SafeHttpRequestBuilder
    {
        private readonly HttpClient _httpClient;
        private readonly RateLimiter _rateLimiter;

        public SafeHttpRequestBuilder(TimeSpan delayBetweenRequests)
        {
            _httpClient = new HttpClient();
            _httpClient.Timeout = TimeSpan.FromSeconds(30);
            _rateLimiter = new RateLimiter(delayBetweenRequests);
        }

        public async Task<HttpResponseMessage> SendRequestAsync(HttpRequestMessage request, CancellationToken cancellationToken = default)
        {
            // Проверка целевого домена
            DomainValidator.ValidateTargetDomain(request.RequestUri.Host);
            
            // Соблюдение ограничения частоты запросов
            await _rateLimiter.WaitAsync();
            
            // Установка безопасных заголовков
            request.Headers.UserAgent.ParseAdd("SecurityTrainingTool/1.0 (Educational)");
            request.Headers.Referrer = new Uri("http://test.example/");
            
            try
            {
                return await _httpClient.SendAsync(request, cancellationToken);
            }
            catch (TaskCanceledException)
            {
                throw new Exception("Запрос отменен из-за таймаута");
            }
            catch (HttpRequestException ex)
            {
                throw new Exception($"Ошибка сетевого запроса: {ex.Message}");
            }
        }

        public void Dispose()
        {
            _httpClient?.Dispose();
        }
    }
}
