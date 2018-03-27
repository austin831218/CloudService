using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace CloudService.Service.WebApi
{
    [Route("api/[controller]")]
    public class TestController : Controller
    {
        [HttpGet]
        public async Task<IActionResult> Get() => Ok(new[] { "value1", "value2" });
    }
}
