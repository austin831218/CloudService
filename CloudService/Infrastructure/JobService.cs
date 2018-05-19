using CloudService.Queues;
using CloudService.Scheduler;
using System.Threading;
using System.Collections.Generic;
using CloudService.Job;
using System.Collections.Concurrent;
using NLog;
using System.Threading.Tasks;
using System;
using System.Linq;
using Autofac;

namespace CloudService.Infrastructure
{
    internal interface IJobService
    {
    }

    internal class JobService : IJobService
    {
        private readonly ILogger _logger = LogManager.GetLogger("JobService", typeof(JobService));
        private readonly IEnumerable<IJobDescriber> _describers;
        private readonly IQueue _q;
        private readonly IServiceContext _context;
        private readonly IJobWorkerManager _workerManager;
        private CancellationTokenSource _cancelTokenSource;
        private ILifetimeScope _scope;
        private List<ISignal> _decreaseSignalHolder;

        public JobService(IEnumerable<IJobDescriber> describers, IQueue q, IServiceContext context, IJobWorkerManager workerManager, ILifetimeScope scope)
        {
            _describers = describers;
            _q = q;
            _context = context;
            _decreaseSignalHolder = new List<ISignal>();
            _workerManager = workerManager;
            _scope = scope;
        }


        public void Start()
        {
            if (_cancelTokenSource != null && !_cancelTokenSource.IsCancellationRequested)
            {
                _logger.Warn($"JobService has already started");
                return;
            }
            _cancelTokenSource = new CancellationTokenSource();
            var token = _cancelTokenSource.Token;
            Task.Factory.StartNew((tk) =>
            {
                _logger.Warn($"started");
                var ct = (CancellationToken)tk;
                while (!ct.IsCancellationRequested)
                {
                    var signal = _q.DequeueOrWait(TimeSpan.FromSeconds(30));
                    var job = getJobDescriber(signal.JobName);
                    if (job == null)
                    {
                        _logger.Warn($"unknown job {signal.JobName}, ignored");
                    }
                    if (job.JobType != JobType.Scheduled) // scheduled job is maintained by schedule manager
                    {
                        if (signal.Type == SignalType.DecreaseJobThread)
                        {
                            _decreaseSignalHolder.Add(signal);
                        }
                        else if (signal.Type == SignalType.JobCompleted || signal.Type == SignalType.JobError)
                        {
                            //TODO: history, broadcast job status
                            if (!consumeDesceaseSignal(signal))
                            {
                                //if no desrease signal detected, add back to Q
                                _q.Enqueue(new Signal(SignalType.JobScheduled, signal.JobName, null), 1);
                            }
                        }
                        else
                        {
                            _workerManager.StartNew(_scope, job, _cancelTokenSource.Token, _context, _q);
                        }
                    }
                }
                _logger.Warn($"stopped");
            }, token, token);
        }

        private bool consumeDesceaseSignal(ISignal completedSignal)
        {
            var d = _decreaseSignalHolder.FirstOrDefault(x => x.JobName == completedSignal.JobName);
            if (d != null)
            {
                _decreaseSignalHolder.Remove(d);
                return true;
            }
            return false;
        }

        private IJobDescriber getJobDescriber(string jobName)
        {
            var d = _describers.FirstOrDefault(x => x.Name == jobName);
            if (d == null) return null;
            return d;
        }
    }
}
