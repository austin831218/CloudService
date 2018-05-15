using CloudService.Queues;
using CloudService.Scheduler;
using System.Threading;

namespace CloudService.Infrastructure
{
    internal interface IJobService
    {
    }

    internal class JobService : IJobService
    {
        private readonly ScheduleManager _scheduleManager;
        private readonly IQueue _q;
        private CancellationTokenSource _cancelTS;
        public JobService(ScheduleManager scheduleManager, IQueue q)
        {
            _scheduleManager = scheduleManager;
            _q = q;
        }


        public void Start()
        {
            
        }
    }
}
