using BrowserInfoService.Models;
using BrowserInfoService.Extensions;

namespace BrowserInfoService.Services
{
    public class BrowserInfoService : IBrowserInfoService
    {
        public BrowserInfo GetBrowserInfo(HttpContext context)
        {
            var request = context.Request;
            
            return new BrowserInfo
            {
                UserAgent = request.Headers.UserAgent.ToString() ?? string.Empty,
                AcceptLanguage = request.Headers.AcceptLanguage.ToString() ?? string.Empty,
                AcceptEncoding = request.Headers.AcceptEncoding.ToString() ?? string.Empty,
                Connection = request.Headers.Connection.ToString() ?? string.Empty,
                CacheControl = request.Headers.CacheControl.ToString() ?? string.Empty,
                SecFetchDest = request.Headers["Sec-Fetch-Dest"].ToString() ?? string.Empty,
                RemoteIp = context.GetRemoteIpAddress()
            };
        }
    }
}
