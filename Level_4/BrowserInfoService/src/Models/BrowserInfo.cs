namespace BrowserInfoService.Models
{
    public class BrowserInfo
    {
        public string UserAgent { get; set; } = string.Empty;
        public string AcceptLanguage { get; set; } = string.Empty;
        public string AcceptEncoding { get; set; } = string.Empty;
        public string Connection { get; set; } = string.Empty;
        public string CacheControl { get; set; } = string.Empty;
        public string SecFetchDest { get; set; } = string.Empty;
        public string RemoteIp { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    }
}
