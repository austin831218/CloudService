using System;
using System.IO;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using CloudService.Common;
using CloudService.Common.Configuration;

namespace CloudService.Host
{
    class Program
    {
        const string Environment = "Development";
        static void Main(string[] args)
        {
            var host = BuildWebHost(args);

            var appCfg = DependencyResolver.Resolve<AppSettings>();

            //TODO: launch services

            host.Run();
        }

        static IWebHost BuildWebHost(string[] args) =>
            WebHost.CreateDefaultBuilder()
                .UseEnvironment(Environment)
                .UseStartup<Startup>()
                .Build();
    }
}
