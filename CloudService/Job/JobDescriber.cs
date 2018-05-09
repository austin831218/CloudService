using System;
using NCrontab;

namespace CloudService.Job
{
	public class JobDescriber
	{
		public string Name { get; set; }
		public int RequestThreads { get; set; }
		public CrontabSchedule Schedule { get; set; }
		public DateTime? LastScheduledTime { get; set; }
		public DateTime NextTime { get; set; }
		public JobType JobType { get; set; }

		public JobDescriber()
		{
		}
	}
}