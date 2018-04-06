using System;
using System.IO;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using CloudService.Common;
using CloudService.Common.Configuration;
using CloudService.Service.WorkTask;
using CloudService.Test;

namespace CloudService.Host
{
    class Program
    {
        const string Environment = "Development";
        static void Main(string[] args)
        {
            var host = BuildWebHost(args);

            //var appCfg = DependencyResolver.Resolve<AppSettings>();
            BuildWorkTasks();

            host.Run();
        }

        static IWebHost BuildWebHost(string[] args) =>
            WebHost.CreateDefaultBuilder()
                .UseEnvironment(Environment)
                .UseStartup<Startup>()
                .Build();

        static void BuildWorkTasks()
        {
            //TODO: launch services
            var settings = new WorkTaskSettings { Interval = 100, WorkerCount = 5 };
            var myTest = new MyTestWorker("MyWorkTask", settings);
            myTest.Start();
            var taskManager = DependencyResolver.Resolve<TaskManager>();
            taskManager.WorkTasks.Add(myTest);
        }
    }
}
