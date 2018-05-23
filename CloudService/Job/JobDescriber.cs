using System;
using System.Collections.Generic;
using System.Text;
using CloudService.Job.Scheduler;
using NCrontab;

namespace CloudService.Job
{
    internal interface IJobDescriber
    {
        string Name { get; }
        string InternalName { get; }
        int RequestThreads { get; set; }
        ISchedule Cron { get; set; }
        JobType JobType { get; }
    }

    internal class JobDescriber<T> : IJobDescriber where T : IJob
    {
        public string Name { get; private set; }
        public string InternalName { get; private set; }
        public int RequestThreads { get; set; }
        public ISchedule Cron { get; set; }
        public JobType JobType { get; private set; }


        public JobDescriber(string name, int requestThreads, string cronExpress, JobType jobType, string internalName)
        {
            this.Name = name;
            this.InternalName = internalName;
            this.RequestThreads = requestThreads;
            this.JobType = jobType;
            if (this.JobType == JobType.Scheduled)
            {
                this.Cron = new Schedule(cronExpress);
            }
        }
    }
}
