using CloudService.Host;
using CloudService.Messaging;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using CloudService.Messaging.Middlewares.WebsocketConsoleMiddleware;

namespace CloudService.Controllers
{
    [Route("test")]
    internal class TestController : Controller
    {
        ServiceHost _host;
        public TestController(ServiceHost host, WebSocketMessageBroadcaster brocaster)
        {
            _host = host;
        }

        [HttpGet("")]
        public IActionResult Test()
        {
            return Ok("<ul>" + string.Join($"\r\n", _host.Services.Select(x => "<li>" + x.ServiceType.FullName + "</li>")) + "</ul>");
        }

    }
}
