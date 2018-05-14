using CloudService.Infrastructure;
using CloudService.Job;
using CloudService.Queues;
using NLog;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CloudService.Scheduler
{
    internal interface IJobScheduler
    {
        IJobDescriber Describer { get; }
        void Start();
        void Stop();
    }

    internal class JobScheduler<T> : IJobScheduler where T : IJob
    {
        public IJobDescriber Describer { get; private set; }
        private CancellationTokenSource _cancelTokenSource;
        private readonly Logger _logger = LogManager.GetCurrentClassLogger();
        private IQueue _queue;
        public JobScheduler(IJobDescriber describer, IQueueManager qMan)
        {
            Describer = describer;
            _queue = qMan.GetQueue("job");
        }

        public void Start()
        {
            _logger.Info($"{Describer.JobType.ToString()} job {Describer.Name} is being scheduled");
            if (Describer.JobType == JobType.Scheduled)
            {
                if (_cancelTokenSource != null && !_cancelTokenSource.IsCancellationRequested)
                {
                    _logger.Warn($"scheduled job {Describer.Name} has already been scheduling");
                    return; // in-case Start called multiple times without Stop called
                }
                _cancelTokenSource = new CancellationTokenSource();
                var token = _cancelTokenSource.Token;
                var lastRun = Describer.Cron.LastScheduledTime ?? DateTime.UtcNow;
                Describer.Cron.NextTime = Describer.Cron.Crontab.GetNextOccurrence(lastRun);
                Task.Factory.StartNew((tk) =>
                {
                    var ct = (CancellationToken)tk;
                    while (!ct.IsCancellationRequested)
                    {
                        _logger.Trace("wait job {0}, last {1}, next {2}", Describer.Name,
                                      Describer.Cron.LastScheduledTime.HasValue ? Describer.Cron.LastScheduledTime.ToString() : "none",
                                      Describer.Cron.NextTime.ToString());
                        var now = DateTime.UtcNow;
                        if (now >= Describer.Cron.NextTime)
                        {
                            Describer.Cron.NextTime = Describer.Cron.Crontab.GetNextOccurrence(now);
                            Describer.Cron.LastScheduledTime = now;
                            _queue.Enqueue(Describer.Name, Describer.RequestThreads);
                        }
                        Thread.Sleep(100);
                    }
                }, token, token);
            }
            else
            {
                _queue.Enqueue(Describer.Name, Describer.RequestThreads);
            }
        }


        public void Stop()
        {
            _logger.Warn($"stop scheduling job {Describer.Name}");
            if (_cancelTokenSource != null && !_cancelTokenSource.IsCancellationRequested)
            {
                _cancelTokenSource.Cancel();
            }
        }
    }
}
