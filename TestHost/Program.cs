﻿using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.Server;
using System;
using System.IO;
using CloudService.Host;
using TestHost.Jobs;

namespace TestHost
{
    class Program
    {
        static void Main(string[] args)
        {
            var host = new WebHostBuilder().UseKestrel()
            .UseCloudService(s =>
            {
                s.ScheduleJob<TestCronJob1>("cron1", 2, "0 0/1 * * * *")
                    .AddRepeatingJob<RepeatingJob1>("rep1", 3)
                    .OnDuty<TestCronJob1>("cron1")
                    .OnDuty<RepeatingJob1>("rep1").BuildServies();
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
