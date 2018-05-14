using System;
using System.Collections.Generic;
using System.Text;
using CloudService.Job.Scheduler;
using NCrontab;

namespace CloudService.Job
{
    internal interface IJobDescriber
    {
        string Name { get; set; }
        int RequestThreads { get; set; }
        ISchedule Cron { get; set; }
        JobType JobType { get; set; }
    }

    internal class JobDescriber<T> : IJobDescriber where T : IJob
    {
        public string Name { get; set; }
        public int RequestThreads { get; set; }
        public ISchedule Cron { get; set; }
        public JobType JobType { get; set; }


        public JobDescriber(string name, int requestThreads, string cronExpress, JobType jobType)
        {
            this.Name = name;
            this.RequestThreads = requestThreads;
            this.JobType = jobType;
            if (this.JobType == JobType.Scheduled)
            {
                this.Cron = new Schedule(cronExpress);
            }
        }
    }
}
