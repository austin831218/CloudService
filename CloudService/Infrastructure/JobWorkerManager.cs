using CloudService.Job;
using System;
using System.Collections.Concurrent;
using System.Linq;
using Autofac;
using CloudService.Queues;
using System.Threading;
using CloudService.Host;
using CloudService.Messaging;
using System.Threading.Tasks;

namespace CloudService.Infrastructure
{
    internal interface IJobWorkerManager
    {
        ConcurrentDictionary<Guid, JobWorker> Workers { get; }
        JobWorker StartNew(ILifetimeScope scope,
            IJobDescriber describer,
            CancellationToken tk,
            IQueue q,
            IHistoryStore hs);

    }

    internal class JobWorkerManager : IJobWorkerManager
    {
        public ConcurrentDictionary<Guid, JobWorker> Workers { get; private set; }
        private readonly IHostConifuration _cfg;
        private readonly SemaphoreSlim _ss;
        private bool _capacityChanging = false;

        public JobWorkerManager(IHostConifuration cfg)
        {
            _cfg = cfg;
            _ss = new SemaphoreSlim(_cfg.Capacity, _cfg.MaxCapacity);
            Workers = new ConcurrentDictionary<Guid, JobWorker>();
        }
        public JobWorker StartNew(ILifetimeScope scope,
            IJobDescriber describer,
            CancellationToken tk,
            IQueue q,
            IHistoryStore hs)
        {
            _ss.Wait();
            var context = new JobContext(describer, q, hs, Guid.NewGuid());
            var worker = new JobWorker(scope, describer, tk, q, context, hs);
            worker.Work(w =>
            {
                Workers.TryRemove(w.ID, out JobWorker k);
                _ss.Release();
            });
            Workers.TryAdd(worker.ID, worker);
            return worker;
        }

        public bool ChangeCapacity(int count)
        {
            if (count > _cfg.MaxCapacity || count < 0)
            {
                return false;
            }
            if (_capacityChanging)
            {
                return false;
            }
            else
            {
                _capacityChanging = true;
            }


            var c = count - _cfg.Capacity;
            _cfg.Capacity = count;
            if (c > 0)
            {
                _ss.Release(c);
            }
            else
            {
                for (var i = 0; i < c; i++)
                {
                    _ss.Wait(0);
                }
            }
            _capacityChanging = false;
            return true;

        }


    }
}
