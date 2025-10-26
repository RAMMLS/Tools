using Microsoft.AspNetCore.Mvc;
using BrowserInfoService.Services;
using BrowserInfoService.Models;

namespace BrowserInfoService.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class BrowserInfoController : ControllerBase
    {
        private readonly IBrowserInfoService _browserInfoService;

        public BrowserInfoController(IBrowserInfoService browserInfoService)
        {
            _browserInfoService = browserInfoService;
        }

        [HttpGet]
        public ActionResult<BrowserInfo> Get()
        {
            return Ok(_browserInfoService.GetBrowserInfo(HttpContext));
        }
    }
}
