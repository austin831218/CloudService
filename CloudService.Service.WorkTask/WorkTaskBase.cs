using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using CloudService.Common;
using CloudService.Common.Configuration;

namespace CloudService.Service.WorkTask
{
    public abstract class WorkTaskBase<T>
    {
        private AppSettings _cfg;
        private ILogger _log;
        private SemaphoreSlim _signal;
        private CancellationTokenSource _cancelSource;
        private Task _t;
        private bool _running = false;

        public DateTime LastPerformedTime { get; private set; }
        public WorkTaskBase()
        {
            _cfg = DependencyResolver.Resolve<AppSettings>();
            _log = DependencyResolver.Resolve<ILogger<WorkTaskBase<T>>>();
            _cancelSource = new CancellationTokenSource();
            _signal = new SemaphoreSlim(_cfg.InitialThreadCount, _cfg.MaxThreadCount);
        }
        protected void Start()
        {
            _running = true;
            Run();
        }
        protected void Start(Action onStart)
        {
            onStart.Invoke();
            _running = true;
            Run();
        }

        protected Task Stop()
        {
            _running = false;
            return _t;
        }
        protected Task Stop(Action onStop)
        {
            onStop.Invoke();
            _running = false;
            return _t;
        }

        protected abstract IEnumerable<T> PrepareTask();
        protected abstract void Execute(T task);
        protected virtual void OnException(T task, Exception ex) { }

        private void Run()
        {
            _t = Task.Factory.StartNew(async () =>
            {
                while (_running)
                {
                    try
                    {
                        var tasks = PrepareTask();
                        if (tasks.Any())

                            foreach (T task in tasks)
                            {
                                _signal.Wait();
                                var t = Task.Factory.StartNew(() =>
                                {
                                    try
                                    {
                                        Execute(task);
                                    }
                                    catch (Exception ex)
                                    {
                                        OnException(task, ex);
                                    }
                                    finally
                                    {
                                        LastPerformedTime = DateTime.Now;
                                        _signal.Release();
                                    }

                                });
                            }
                    }
                    finally
                    {
                        await Task.Delay(_cfg.ThreadSuspend, _cancelSource.Token);
                    }
                }
            });
        }
    }
}
