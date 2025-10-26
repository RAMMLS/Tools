using BrowserInfoService.Models;
using BrowserInfoService.Extensions;

namespace BrowserInfoService.Services {
  public class BrowserInfoService : IBrowserInfoService {
    public BrowserInfo GetBrowserInfo(HttpContext context) {
      return new BrowserInfo {
        UserAgent = context.Request.Headers.UserAgent,
        AcceptLanguage = context.Request.Headers.AcceptLanguage,
        AcceptEncoding = context.Request.Headers.AcceptEncoding,
        Connection = context.Request.Headers.Connection,
        CacheControl = context.Request.Headers.CacheControl,
        SecFetchDest = context.Request.Headers.SecFetchDest,
        RemoteIp = context.GetRemoteIpAddress()
      };
    }
  }
}
