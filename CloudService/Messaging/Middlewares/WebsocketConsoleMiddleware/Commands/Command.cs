using System;
using System.Collections.Generic;
using System.Text;

namespace CloudService.Messaging.Middlewares.WebsocketConsoleMiddleware.Commands
{
    internal class Command
    {
        public CommandType Type { get; set; }
        public string JobName { get; set; }
        public Guid WorkerId { get; set; }
        public int Count { get; set; }
    }
}
