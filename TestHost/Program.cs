using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.Server;
using System;
using System.IO;
using CloudService.Host;

namespace TestHost
{
    class Program
    {
        static void Main(string[] args)
        {
            var host = new WebHostBuilder().UseKestrel()
            .UseCloudService(s =>
            {
                s.BuildServies();
            })
            .UseContentRoot(Directory.GetCurrentDirectory())
            .UseStartup<Startup>()
            .UseUrls("http://*:10000")
            .Build();

            host.Run();
            var ch = host.Services.GetService(typeof(ServiceHost)) as ServiceHost;
        }
    }
}
