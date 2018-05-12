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

namespace CloudService.Host
{
    public class ServiceHost
    {
        private List<JobDescriber> _jobFeatures;
        private List<Action<ContainerBuilder>> _buildActions;
        public IServiceProvider Container { get; set; }
        public IServiceCollection Services { get; private set; }

        public ServiceHost(IServiceCollection services)
        {
            Services = services;
            _jobFeatures = new List<JobDescriber>();
            this._buildActions = new List<Action<ContainerBuilder>>();
            _buildActions.Add(b =>
           {
               b.Register(c => this).SingleInstance();
           });
            _buildActions.Add(b => b.Register<JobManager>(c => new JobManager(_jobFeatures, c.Resolve<IJobQueue>())).SingleInstance());
            _buildActions.Add(b => b.RegisterType<MemoryJobQueue>().As<IJobQueue>().SingleInstance());
            _buildActions.Add(b => b.RegisterType<MemoryHistoryStore>().As<IHistoryStore>().SingleInstance());
            _buildActions.Add(b => b.RegisterType<WebSocketMessageBroadcaster>().As<IMessageBroadcaster>().SingleInstance());
        }

        public void RegisterRepeatingJob<T>(string name, int requestThreads)
        {
            if (_jobFeatures.Any(x => string.Compare(name, x.Name, true) == 0))
            {
                throw new Exception($"Duplicated Job Names {name}");
            }
            _jobFeatures.Add(new JobDescriber
            {
                Name = name,
                RequestThreads = requestThreads,
                JobType = JobType.Repeating
            });
            _buildActions.Add(b => b.RegisterType(typeof(T)).Named<IJob>(name));
        }

        public void RegisterLongRunningJob<T>(string name, int requestThreads)
        {
            if (_jobFeatures.Any(x => string.Compare(name, x.Name, true) == 0))
            {
                throw new Exception($"Duplicated Job Names {name}");
            }
            _jobFeatures.Add(new JobDescriber
            {
                Name = name,
                RequestThreads = requestThreads,
                JobType = JobType.LongRunning
            });
            _buildActions.Add(b => b.RegisterType(typeof(T)).Named<IJob>(name));
        }

        public void RegisterScheduledJob<T>(string name, string cronExpression, int requestThreads)
        {
            if (_jobFeatures.Any(x => string.Compare(name, x.Name, true) == 0))
            {
                throw new Exception($"Duplicated Job Names {name}");
            }

            var schedule = CrontabSchedule.Parse(cronExpression, new CrontabSchedule.ParseOptions
            {
                IncludingSeconds = true
            });
            _jobFeatures.Add(new JobDescriber
            {
                Name = name,
                RequestThreads = requestThreads,
                JobType = JobType.Scheduled,
                Schedule = schedule
            });
            _buildActions.Add(b => b.RegisterType(typeof(T)).Named<IJob>(name));
        }

        public void BuildServies()
        {
            this.Services.AddAutofac(b => this._buildActions.ForEach(a => a.Invoke(b)));
        }
    }
}
