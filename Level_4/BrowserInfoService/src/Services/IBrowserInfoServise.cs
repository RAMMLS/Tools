using BrowserInfoService.Models {
  namespace BrowserInfoService.Services {
    public interface IBrowserInfoService {
      BrowserInfo GetBrowserInfo(HttpContext context);
    }
  }
}
