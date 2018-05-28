using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace CloudService.Messaging.Middlewares.WebsocketConsoleMiddleware
{
    internal static class WebsocketConsoleMiddlewareExtension
    {
        public static IApplicationBuilder MapBroadcaster(this IApplicationBuilder app,
                                                            PathString path,
                                                            WebsocketConsoleHandler handler)
        {
            return app.Map(path, (_app) => _app.UseMiddleware<Middleware>(handler));
        }
    }
}
