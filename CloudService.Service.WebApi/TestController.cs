using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using CloudService.Service.WorkTask;
using CloudService.Common;

namespace CloudService.Service.WebApi
{
    //[Route("api/[controller]")]
    public class TestController : Controller
    {
        private ILogger _log;
        public TestController(ILogger<TestController> log)
        {
            _log = log;
        }
        [HttpGet]
        [Route("api/Types")]
        public async Task<IActionResult> WorkTypes()
        {
            _log.LogInformation("get work types");

            var taskManager = DependencyResolver.Resolve<TaskManager>();

            return Ok(taskManager.WorkTasks.Select(t => t.Name));
        }

        [HttpGet]
        [Route("api/Tasks")]
        public async Task<IActionResult> GetWorkerTasks([FromQuery]string type)
        {
            _log.LogInformation($"get work tasks for type {type}");

            var taskManager = DependencyResolver.Resolve<TaskManager>();

            var work = taskManager.WorkTasks.Where(t => t.Name == type).FirstOrDefault();
            if (work != null)
                return Ok(work.Workers.Select(w=>w.Value));
            else
                return Ok();
        }

        [HttpPost]
        [Route("api/Tasks/{type}/Add")]
        public async Task<IActionResult> AddWorkerTasks([FromQuery]string type, [FromBody]int count)
        {
            _log.LogInformation($"Add work tasks for type {type}");

            var taskManager = DependencyResolver.Resolve<TaskManager>();

            var work = taskManager.WorkTasks.Where(t => t.Name == type).FirstOrDefault();
            if (work != null)
                work.IncreaseWorkerCount(count);
            return Ok();
        }
    }
}
