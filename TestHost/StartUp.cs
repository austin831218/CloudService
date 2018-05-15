using CloudService.Host;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace TestHost
{
    public class Startup : ServieHostStartupBase
    {
        public Startup(IHostingEnvironment env) : base(env)
        {

        }

        public override void Configure(IApplicationBuilder app, ILoggerFactory loggerFactory, IApplicationLifetime appLifetime)
        {
            base.Configure(app, loggerFactory, appLifetime);
        }
    }
}
