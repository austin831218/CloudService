using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;
using CloudService.Job.Scheduler;
using NCrontab;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

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
    [DataContract]
    internal class JobDescriber<T> : IJobDescriber where T : IJob
    {
        [DataMember]
        public string Name { get; private set; }
        [DataMember]
        public string InternalName { get; private set; }
        [DataMember]
        public int RequestThreads { get; set; }
        [DataMember]
        public ISchedule Cron { get; set; }

        [DataMember]
        [JsonConverter(typeof(StringEnumConverter))]
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
