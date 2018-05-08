using System;

namespace CloudService.Job
{
	public class JobFeature
	{
		public string Name { get; set; }
		public int RequestThreads { get; set; }
		public string CronExpression { get; set; }
		public JobType JobType { get; set; }

		public JobFeature()
		{
		}
	}
}