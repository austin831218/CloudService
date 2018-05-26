using CloudService.Host;
using CloudService.Infrastructure;
using CloudService.Job;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace CloudService.Messaging
{
    internal class ServiceStatics
    {
        public int Capacity { get; set; }
        public int MaxCapacity { get; set; }
        public int Available { get; set; }
        public List<JobStattics> Jobs { get; set; }

        public ServiceStatics(IEnumerable<IJobDescriber> describers, ConcurrentDictionary<Guid, JobWorker> Workers, IHostConifuration cfg, int available)
        {
            this.Capacity = cfg.Capacity;
            this.MaxCapacity = cfg.MaxCapacity;
            this.Available = available;
            this.Jobs = new List<JobStattics>();
            foreach (var d in describers)
            {
                var js = new JobStattics()
                {
                    Describer = d,
                    Workers = Workers.Where(x => x.Value.Describer.Name == d.Name).Select(x => x.Key).ToList()
                };
                this.Jobs.Add(js);
            }
        }

        public IMessage GetMessage()
        {
            var msg = new Message
            {
                Type = MessageType.Statics,
                Data = this
            };
            return msg;
        }
    }

    internal class JobStattics
    {
        public IJobDescriber Describer { get; set; }
        public List<Guid> Workers { get; set; }

    }
}
