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
        private readonly IJobDescriber _describer;
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
            _describer = describer;
            _linkedCTS = CancellationTokenSource.CreateLinkedTokenSource(tk);
            _q = q;
            _context = context;
            _logger = LogManager.GetLogger(_describer.Name);
        }

        public void Work(Action<JobWorker> callback)
        {
            Task.Factory.StartNew(() => this.work(), _linkedCTS.Token).ContinueWith(t =>
            {
                callback?.Invoke(this);
            });
        }

        private void work()
        {
            using (var scope = _scope.BeginLifetimeScope())
            {
                IJob job = null;
                try
                {
                    job = scope.ResolveNamed<IJob>(_describer.InternalName);
                }
                catch (Exception ex)
                {

                    _context.Fatal(ex, $"Can't resolve job {_describer.InternalName} {_describer.Name}");
                }
                try
                {
                    _context.Info($"Job {_describer.InternalName} {_describer.Name} staring");
                    job.Execute(_context, _linkedCTS.Token);
                    _context.Info($"Job {_describer.InternalName} {_describer.Name} execute completed");
                    _q.Enqueue(new Signal(SignalType.JobCompleted, _describer.Name, _describer.InternalName), 1);
                }
                catch (Exception ex)
                {
                    _context.Error(ex, $"Execute job {_describer.InternalName} {_describer.Name} failed");
                    _q.Enqueue(new Signal(SignalType.JobError, _describer.Name, ex.Message), 1);
                }

            }
        }
    }
}
