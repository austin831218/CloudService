using CloudService.Queues;
using CloudService.Scheduler;
using System;
namespace CloudService.Infrastructure
{
    internal interface IJobService
    {
    }

    internal class JobService : IJobService
    {
        private readonly ScheduleManager _scheduleManager;
        private readonly IQueue _q;
        public JobService(ScheduleManager scheduleManager, IQueue q)
        {
            _scheduleManager = scheduleManager;
            _q = q;
        }


        public void Start()
        {
            _scheduleManager.Start();

        }
    }
}
