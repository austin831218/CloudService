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

namespace CloudService.Host
{
	public class HostBase
	{
		public static IContainer Container => _container;
		private readonly Logger _logger = LogManager.GetCurrentClassLogger();


		private static IContainer _container;
		private ContainerBuilder _containerBuilder;

		private Action<IServiceCollection> _serviceOption;
		private Action<ContainerBuilder> _builderOption;
		private Action<IApplicationBuilder> _appBuilderOption;
		private List<JobFeature> _jobFeatures;

		public HostBase(IHostingEnvironment env)
		{
			_containerBuilder = new ContainerBuilder();
			_jobFeatures = new List<JobFeature>();
			var builder = new ConfigurationBuilder()
			   .SetBasePath(env.ContentRootPath)
			   .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
			   .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
			   .AddEnvironmentVariables();
			var cfg = builder.Build();
			_containerBuilder.Register(c => cfg).As<IConfigurationRoot>();
		}


		public IServiceProvider ConfigureServices(IServiceCollection services)
		{
			if (_serviceOption != null)
			{
				_serviceOption.Invoke(services);
			}
			if (_builderOption != null)
			{
				_builderOption.Invoke(_containerBuilder);
			}
			_containerBuilder.Populate(services);
			_containerBuilder.Register<JobManager>(c=> new JobManager(_jobFeatures)).SingleInstance();

			_container = _containerBuilder.Build();
			return new AutofacServiceProvider(_container);
		}

		public void Configure(IApplicationBuilder app, ILoggerFactory loggerFactory, IApplicationLifetime appLifetime)
		{
			loggerFactory.AddNLog();
			if (_appBuilderOption != null)
			{
				_appBuilderOption.Invoke(app);
			}
			appLifetime.ApplicationStopped.Register(() => _container.Dispose());
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="builderOpt">autofac container builder, register custom services</param>
		/// <param name="serviceOpt">.net core web app service collection, configure MVC, json, filters, etc. </param>
		/// <param name="appBuilderOpt">add middle wares</param>
		public void Config(Action<ContainerBuilder> builderOpt,
			Action<IServiceCollection> serviceOpt,
			Action<IApplicationBuilder> appBuilderOpt)
		{
			_builderOption = builderOpt;
			_serviceOption = serviceOpt;
			_appBuilderOption = appBuilderOpt;
		}




		public void RegisterRepeatingJob<T>(string name, int requestThreads)
		{
			if (_jobFeatures.Any(x => string.Compare(name, x.Name, true) == 0))
			{
				throw new Exception($"Duplicated Job Names {name}");
			}
			_jobFeatures.Add(new JobFeature
			{
				Name = name,
				RequestThreads = requestThreads,
				JobType = JobType.Repeating
			});
			_containerBuilder.RegisterType(typeof(T)).Named<IJob>(name);
		}

		public void RegisterLongRunningJob<T>(string name, int requestThreads)
		{
			if (_jobFeatures.Any(x => string.Compare(name, x.Name, true) == 0))
			{
				throw new Exception($"Duplicated Job Names {name}");
			}
			_jobFeatures.Add(new JobFeature
			{
				Name = name,
				RequestThreads = requestThreads,
				JobType = JobType.LongRunning
			});
			_containerBuilder.RegisterType(typeof(T)).Named<IJob>(name);
		}

		public void RegisterScheduledJob<T>(string name, string cronExpression, int requestThreads)
		{
			if (_jobFeatures.Any(x => string.Compare(name, x.Name, true) == 0))
			{
				throw new Exception($"Duplicated Job Names {name}");
			}
			_jobFeatures.Add(new JobFeature
			{
				Name = name,
				RequestThreads = requestThreads,
				JobType = JobType.Scheduled,
				CronExpression = cronExpression
			});
			_containerBuilder.RegisterType(typeof(T)).Named<IJob>(name);
		}


	}
}
