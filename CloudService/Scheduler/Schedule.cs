using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NCrontab;
using NLog;

namespace CloudService.Job.Scheduler
{
    public interface ISchedule
    {
        CrontabSchedule Crontab { get; set; }
        DateTime? LastScheduledTime { get; set; }
        DateTime NextTime { get; set; }
    }


    internal class Schedule : ISchedule
    {
        public CrontabSchedule Crontab { get; set; }
        public DateTime? LastScheduledTime { get; set; }
        public DateTime NextTime { get; set; }
        private readonly Logger _logger = LogManager.GetCurrentClassLogger();

        public Schedule(string cronExp)
        {
            Crontab = CrontabSchedule.Parse(cronExp, new CrontabSchedule.ParseOptions
            {
                IncludingSeconds = true
            });
        }

    }
}
