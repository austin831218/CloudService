using System;
using System.Reflection;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using CloudService.Job;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using NLog.Extensions.Logging;
using NLog;
using System.Collections.Generic;
using System.Linq;
using CloudService.Infrastructure;
using NCrontab;

namespace CloudService.Host
{
    public class ServieHostStartupBase
    {
        private readonly Logger _logger = LogManager.GetCurrentClassLogger();
        public IConfigurationRoot Configuration { get; private set; }

        public ServieHostStartupBase(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
               .SetBasePath(env.ContentRootPath)
               .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
               .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
               .AddEnvironmentVariables();
            this.Configuration = builder.Build();
        }


        public virtual void ConfigureServices(IServiceCollection services)
        {

        }

        public virtual void ConfigureContainer(ContainerBuilder builder)
        {

        }

        public virtual void Configure(IApplicationBuilder app, ILoggerFactory loggerFactory, IApplicationLifetime appLifetime)
        {
            loggerFactory.AddNLog();
            app.UseMvcWithDefaultRoute();
            //var serviceHost = app.ApplicationServices.GetService<ServiceHost>();
            //serviceHost.Container = app.ApplicationServices;
            //serviceHost.Start();
        }
    }
}
