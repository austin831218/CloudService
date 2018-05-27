using System;
using System.Collections.Generic;
using System.Text;

namespace CloudService.Messaging.Middlewares.WebsocketConsoleMiddleware.Commands
{
    internal enum CommandType
    {
        Ping,
        GetLog,
        ChangeCapacity,
        StopWorker
    }
}
