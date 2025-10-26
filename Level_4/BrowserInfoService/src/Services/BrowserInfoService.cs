using BrowserInfoService.Models;
using BrowserInfoService.Extensions;

namespace BrowserInfoService.Services
{
    public class BrowserInfoCollector : IBrowserInfoService
    {
        private readonly ILogger<BrowserInfoCollector> _logger;

        public BrowserInfoCollector(ILogger<BrowserInfoCollector> logger)
        {
            _logger = logger;
        }

        public BrowserInfo GetBrowserInfo(HttpContext context)
        {
            try
            {
                var request = context.Request;
                
                var browserInfo = new BrowserInfo
                {
                    UserAgent = GetHeaderValue(request.Headers, "User-Agent"),
                    AcceptLanguage = GetHeaderValue(request.Headers, "Accept-Language"),
                    AcceptEncoding = GetHeaderValue(request.Headers, "Accept-Encoding"),
                    Connection = GetHeaderValue(request.Headers, "Connection"),
                    CacheControl = GetHeaderValue(request.Headers, "Cache-Control"),
                    SecFetchDest = GetHeaderValue(request.Headers, "Sec-Fetch-Dest"),
                    RemoteIp = context.GetRemoteIpAddress()
                };

                _logger.LogInformation("Browser info collected for IP: {RemoteIp}", browserInfo.RemoteIp);
                return browserInfo;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error collecting browser info");
                throw;
            }
        }

        private static string GetHeaderValue(IHeaderDictionary headers, string key)
        {
            return headers.TryGetValue(key, out var value) ? value.ToString() : string.Empty;
        }
    }
}
