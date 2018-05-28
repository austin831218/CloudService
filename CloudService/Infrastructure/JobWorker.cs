using System;
using System.Threading;
using System.Threading.Tasks;
using Autofac;
using CloudService.Job;
using CloudService.Queues;
using NLog;
using System.Collections.Concurrent;
using CloudService.Messaging;

namespace CloudService.Infrastructure
{
    internal class JobWorker
    {
        private readonly ILifetimeScope _scope;
        public IJobDescriber Describer { get; private set; }
        private readonly CancellationTokenSource _linkedCTS;
        private readonly IJobContext _context;
        private readonly IQueue _q;
        private readonly ILogger _logger;
        public Guid ID { get; private set; }

        public JobWorker(ILifetimeScope scope,
            IJobDescriber describer,
            CancellationToken tk,
            IQueue q,
            IJobContext context,
            IHistoryStore hs)
        {
            this.ID = context.WorkerId;
            _scope = scope;
            Describer = describer;
            _linkedCTS = CancellationTokenSource.CreateLinkedTokenSource(tk);
            _q = q;
            _context = context;
            _logger = LogManager.GetLogger(Describer.Name);
        }

        public void Work(Action<JobWorker> callback)
        {
            Task.Factory.StartNew(() => this.work(), _linkedCTS.Token).ContinueWith(t =>
            {
                callback?.Invoke(this);
            });
        }

        public void Stop()
        {
            _context.Warn("stopped by admin");
            _linkedCTS.Cancel();
        }

        private void work()
        {
            using (var scope = _scope.BeginLifetimeScope())
            {
                IJob job = null;
                try
                {
                    job = scope.ResolveNamed<IJob>(Describer.InternalName);
                }
                catch (Exception ex)
                {

                    _context.Fatal(ex, $"Can't resolve job {Describer.InternalName} {Describer.Name}");
                }
                try
                {
                    _context.Info($"Job {Describer.InternalName} {Describer.Name} staring");
                    job.Execute(_context, _linkedCTS.Token);
                    _context.Info($"Job {Describer.InternalName} {Describer.Name} execute completed");
                    _q.Enqueue(new Signal(SignalType.JobCompleted, Describer.Name, Describer.InternalName), 1);
                }
                catch (Exception ex)
                {
                    _context.Error(ex, $"Execute job {Describer.InternalName} {Describer.Name} failed");
                    _q.Enqueue(new Signal(SignalType.JobError, Describer.Name, ex.Message), 1);
                }

            }
        }
    }
}
