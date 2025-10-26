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
        private readonly ILogger<BrowserInfoController> _logger;

        public BrowserInfoController(
            IBrowserInfoService browserInfoService,
            ILogger<BrowserInfoController> logger)
        {
            _browserInfoService = browserInfoService;
            _logger = logger;
        }

        [HttpGet]
        public ActionResult<BrowserInfo> Get()
        {
            try
            {
                var browserInfo = _browserInfoService.GetBrowserInfo(HttpContext);
                _logger.LogInformation("Successfully returned browser info");
                return Ok(browserInfo);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in BrowserInfoController");
                return StatusCode(500, "Internal server error");
            }
        }
    }
}
