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
using CloudService.Messaging.Middlewares.WebsocketConsoleMiddleware;
using System.Collections.Generic;


namespace CloudService.Infrastructure
{
    internal interface IJobWorkerManager
    {
        ConcurrentDictionary<Guid, JobWorker> Workers { get; }
        bool ChangeCapacity(int count);
        void ChangeJobThreads(string JobName, int Count);
        void StopWorker(string id);
        void broadcastServiceStatics();
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
        private readonly WebSocketMessageBroadcaster _broadcaster;
        private readonly IEnumerable<IJobDescriber> _describers;
        private readonly IQueue _q;

        public JobWorkerManager(IHostConifuration cfg,
            WebSocketMessageBroadcaster broadcaster,
            IEnumerable<IJobDescriber> describers,
            IQueue q)
        {
            _q = q;
            _cfg = cfg;
            _broadcaster = broadcaster;
            _describers = describers;
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

            var context = new JobContext(describer, Guid.NewGuid(), _broadcaster);
            var worker = new JobWorker(scope, describer, tk, q, context, hs);
            worker.Work(w =>
            {
                Workers.TryRemove(w.ID, out JobWorker k);
                _ss.Release();
                this.broadcastServiceStatics();
            });
            Workers.TryAdd(worker.ID, worker);
            this.broadcastServiceStatics();
            return worker;
        }

        public void broadcastServiceStatics()
        {
            _broadcaster.BroadcastMessage(new ServiceStatics(_describers, Workers, _cfg, _ss.CurrentCount, _q.Count).GetMessage());
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
            _broadcaster.BroadcastMessage(new Message
            {
                Type = MessageType.Notification,
                Content = $"Server capacity is changed to {count}",
            });
            this.broadcastServiceStatics();
            return true;
        }

        public void ChangeJobThreads(string JobName, int count)
        {
            if (count <= 0) return;
            var jd = _describers.FirstOrDefault(x => x.Name == JobName);
            if (jd == null) return;
            if (jd.JobType == JobType.Scheduled)
            {
                jd.RequestThreads = count; // cron job just need to change the describer, then next schedule will use it                
            }
            else
            {
                var diff = count - jd.RequestThreads;
                if (diff > 0) // increase
                {
                    jd.RequestThreads = count;
                    _q.Enqueue(new Signal(SignalType.JobScheduled, jd.Name, jd.InternalName), diff);
                }
                else
                {
                    _q.Enqueue(new Signal(SignalType.DecreaseJobThread, jd.Name, jd.InternalName), Math.Abs(diff));
                    if (jd.JobType == JobType.LongRunning) // stop the long running job
                    {
                        Workers.Values.Where(x => x.Describer.Name == jd.Name).Take(Math.Abs(diff)).ToList().ForEach(x =>
                         {
                             if (x.Describer.Name == jd.Name)
                             {
                                 x.Stop();
                             }
                         });
                    }
                }
            }
            _broadcaster.BroadcastMessage(new Message
            {
                Type = MessageType.Notification,
                Content = $"Job {jd.Name}'s threads is changed to {count}",
            });
        }

        public void StopWorker(string id)
        {
            if (Guid.TryParse(id, out Guid workerId))
            {
                if (Workers.ContainsKey(workerId))
                {
                    var w = Workers[workerId];
                    var jd = w.Describer;
                    w.Stop();
                    _broadcaster.BroadcastMessage(new Message
                    {
                        Type = MessageType.Notification,
                        Content = $"Worker {workerId.ToString()} of Job {jd.Name} is stopped",
                    });
                }
            }
        }
    }
}
