using System;
using System.Collections.Generic;
using CloudService.Job;

namespace CloudService.Infrastructure
{
	public class JobManager
	{
		private List<JobFeature> _jobFeatures;
		public JobManager(List<JobFeature> jobFeatures)
		{
			_jobFeatures = jobFeatures;
		}
	}
}
