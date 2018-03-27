using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace CloudService.Service.WebApi
{
    [Route("api/[controller]")]
    public class TestController : Controller
    {
        private ILogger _log;
        public TestController(ILogger<TestController> log)
        {
            _log = log;
        }
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            _log.LogInformation("get values");
            return Ok(new[] { "value1", "value2" });
        }
    }
}
