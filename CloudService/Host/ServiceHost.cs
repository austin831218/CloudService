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
using CloudService.Messaging;
using CloudService.Scheduler;
using CloudService.Queues;
using CloudService.Messaging.Middlewares.WebsocketConsoleMiddleware;

namespace CloudService.Host
{
    public class ServiceHost
    {
        private List<Action<ContainerBuilder>> _buildActions;
        public IServiceProvider Container { get; internal set; }
        public IServiceCollection Services { get; private set; }


        public ServiceHost(IServiceCollection services)
        {
            Services = services;
            this._buildActions = new List<Action<ContainerBuilder>>();
            _buildActions.Add(b =>
           {
               b.Register(c => this).SingleInstance();
           });
            _buildActions.Add(b =>
            {
                b.RegisterType<ScheduleManager>().SingleInstance();
                b.RegisterType<MemoryQueue>().As<IQueue>().SingleInstance();
                b.RegisterType<MemoryHistoryStore>().As<IHistoryStore>().SingleInstance();
                b.RegisterType<JobWorkerManager>().As<IJobWorkerManager>().SingleInstance();
                b.Register<IHostConifuration>(c => new HostConfiguration()).SingleInstance();
                b.RegisterType<JobService>().As<IJobService>().SingleInstance();
                b.RegisterGeneric(typeof(ServiceLogger<>)).AsImplementedInterfaces().SingleInstance();
                b.RegisterType<WebSocketMessageBroadcaster>().SingleInstance();
                b.RegisterType<WebsocketConsoleHandler>().SingleInstance();
            });
        }

        public ServiceHost ConfigService(Action<IHostConifuration> configurationOptions)
        {
            var cfg = new HostConfiguration();
            configurationOptions(cfg);
            _buildActions.Add(b => b.Register<IHostConifuration>(c => cfg).SingleInstance());
            return this;
        }

        public ServiceHost ScheduleJob<T>(string name, int requestThreads, string cronExpression) where T : IJob
        {
            this.ScheduleJob<T>(name, requestThreads, cronExpression, JobType.Scheduled);
            return this;
        }
        public ServiceHost AddRepeatingJob<T>(string name, int requestThreads) where T : IJob
        {
            this.ScheduleJob<T>(name, requestThreads, null, JobType.Repeating);
            return this;
        }
        public ServiceHost AddLongRunningJob<T>(string name, int requestThreads) where T : IJob
        {
            this.ScheduleJob<T>(name, requestThreads, null, JobType.LongRunning);
            return this;
        }

        private void ScheduleJob<T>(string name, int requestThreads, string cronExpression, JobType jobType) where T : IJob
        {
            var internalName = typeof(T).FullName;
            CrontabSchedule schedule = null;
            if (jobType == JobType.Scheduled)
            {
                schedule = CrontabSchedule.Parse(cronExpression, new CrontabSchedule.ParseOptions
                {
                    IncludingSeconds = true
                });
            }

            _buildActions.Add(b =>
            {
                b.Register(c => new JobDescriber<T>(name, requestThreads, cronExpression, jobType, internalName)).AsSelf().AsImplementedInterfaces().Named<JobDescriber<T>>(internalName).SingleInstance();
                b.Register(c => new JobScheduler<T>(c.Resolve<JobDescriber<T>>(), c.Resolve<IQueue>())).AsSelf().AsImplementedInterfaces().Named<JobScheduler<T>>(internalName).SingleInstance();
                b.RegisterType(typeof(T)).AsImplementedInterfaces().Named<IJob>(internalName).InstancePerLifetimeScope();
            });
        }



        public void BuildServies()
        {
            this.Services.AddAutofac(b => this._buildActions.ForEach(a => a.Invoke(b)));
        }

        internal bool Start()
        {
            var logger = this.Container.GetService<IServiceLogger<ServiceHost>>();
            logger.Info("CloudService is staring");

            var scheduler = this.Container.GetService<ScheduleManager>();
            scheduler.Start();

            var jobService = this.Container.GetService<IJobService>();
            jobService.Start();

            logger.Info("CloudService is started");
            return true;
        }

        internal void Stop()
        {
            var logger = this.Container.GetService<IServiceLogger<ServiceHost>>();
            logger.Info("CloudService is stopping");

            var scheduler = this.Container.GetService<ScheduleManager>();
            scheduler.Stop();

            var jobService = this.Container.GetService<IJobService>();
            jobService.Stop();
            logger.Info("CloudService is stopped");
        }
    }
}
