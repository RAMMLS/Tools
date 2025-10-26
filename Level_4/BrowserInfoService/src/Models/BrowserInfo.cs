namespace BrowserInfoService.Models {
  public class BrowserInfo {
    public string? UserAgent { get; set; }
    public string? AcceptLanguage { get; set; }
    public string? AcceptEncoding { get; set; }
    public string? Connection { get; set; }
    public string? CacheControle { get; set; }
    public string? SecFetchDest { get; set; }
    public string? RemoteIp { get; set; }
  }
}
