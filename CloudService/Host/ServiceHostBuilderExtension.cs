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
    public static class ServiceHostBuilderExtension
    {
        public static IWebHostBuilder UseCloudService(this IWebHostBuilder builder, Action<ServiceHost> options)
        {
            return builder.ConfigureServices(s =>
            {
                var h = new ServiceHost(s);
                s.AddMvcCore();
                options(h);
                
            });
        }
    }
}
