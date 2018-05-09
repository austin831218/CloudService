using System;
using System.Collections.Generic;
using CloudService.Job;

namespace CloudService.Infrastructure
{
	public class JobManager
	{
		private List<JobDescriber> _jobFeatures;
		private readonly IJobQueue _jobQueue;
		private List<JobScheduler> _arrangers;

		public JobManager(List<JobDescriber> jobFeatures, IJobQueue queue)
		{
			_jobFeatures = jobFeatures;
			_jobQueue = queue;
			_arrangers = new List<JobScheduler>();
		}

		public void ArrangeJobs()
		{
			_jobFeatures.ForEach(f =>
			{
				if (f.JobType == JobType.Scheduled)
				{
					this.arrangeScheduledJobs(f);
				}
				else
				{
					for (var i = 0; i < f.RequestThreads; i++)
					{
						_jobQueue.Enqueue(f.Name);
					}
				}
			});
		}

		private void arrangeScheduledJobs(JobDescriber feature)
		{

		}
	}
}
