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

namespace CloudService.Host
{
    public class ServiceHost
    {
        private List<Action<ContainerBuilder>> _buildActions;
        public IServiceProvider Container { get; set; }
        public IServiceCollection Services { get; private set; }

        public ServiceHost(IServiceCollection services)
        {
            Services = services;
            this._buildActions = new List<Action<ContainerBuilder>>();
            _buildActions.Add(b =>
           {
               b.Register(c => this).SingleInstance();
           });
            _buildActions.Add(b => b.RegisterType<MemoryJobQueue>().As<IJobQueue>().SingleInstance());
            _buildActions.Add(b => b.RegisterType<ScheduleManager>().SingleInstance());
            _buildActions.Add(b => b.RegisterType<MemoryHistoryStore>().As<IHistoryStore>().SingleInstance());
            _buildActions.Add(b => b.RegisterType<WebSocketMessageBroadcaster>().As<IMessageBroadcaster>().SingleInstance());
            _buildActions.Add(b => b.RegisterType<ServiceContext>().As<IServiceContext>().SingleInstance());
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
                b.Register(c => new JobDescriber<T>(name, requestThreads, cronExpression, jobType)).AsSelf().AsImplementedInterfaces().Named<JobDescriber<T>>(name).SingleInstance();
                b.Register(c => new JobScheduler<T>(c.Resolve<JobDescriber<T>>(), c.Resolve<IJobQueue>())).AsSelf().AsImplementedInterfaces().Named<JobScheduler<T>>(name).SingleInstance();
            });
        }

        public ServiceHost OnDuty<T>(string name) where T : IJob
        {
            _buildActions.Add(b =>
            {
                b.RegisterType(typeof(T)).AsImplementedInterfaces().Named<IJob>(name).InstancePerLifetimeScope();
            });
            return this;
        }

        public void BuildServies()
        {
            this.Services.AddAutofac(b => this._buildActions.ForEach(a => a.Invoke(b)));
        }
    }
}
