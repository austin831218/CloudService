using System;
using System.Collections.Generic;
using System.Linq;

namespace CloudService.Scheduler
{
    internal class ScheduleManager
    {
        public List<IJobScheduler> Schedules { get; private set; }

        public ScheduleManager(IEnumerable<IJobScheduler> schedulers)
        {
            Schedules = schedulers.ToList();
        }

        public void Start()
        {
            this.Schedules.ForEach(s => s.Start());
        }

        public void Stop()
        {
            this.Schedules.ForEach(s => s.Stop());
        }
    }
}
