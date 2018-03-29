using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using CloudService.Infrastructure.Exceptions;
using CloudService.Model;

namespace CloudService.Service.WorkTask
{
    public abstract class WorkTaskBase<T> : IConfigurable
    {
        const int _maxWorkerCount = 1000;
        private ILogger<WorkTaskBase<T>> _log;
        private SemaphoreSlim _signal;
        private CancellationTokenSource _cancelSource;
        private Task _t;
        private bool _running = false;
        private string _name = "Worker";
        private int _interval = 100;

        public DateTime LastPerformedTime { get; private set; }
        public IList<Worker> Workers { get; private set; }
        protected string Name
        {
            get => _name;
            set => _name = value;
        }

        protected WorkTaskBase()
        {
            _cancelSource = new CancellationTokenSource();
            _signal = new SemaphoreSlim(1, _maxWorkerCount);
        }
        protected WorkTaskBase(WorkTaskSettings<WorkTaskBase<T>> settings)
        {
            _log = settings.Logger;
            _interval = settings.Interval;
            _cancelSource = new CancellationTokenSource();
            _signal = new SemaphoreSlim(settings.WorkerCount, _maxWorkerCount);
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
        public Task Stop()
        {
            _running = false;
            return _t;
        }
        public Task Stop(Action onStop)
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

                                Workers.Add(new Worker(Name, t));
                            }
                    }
                    finally
                    {
                        await Task.Delay(_interval, _cancelSource.Token);
                    }
                }
            });
        }
        public void IncreaseWorkerCount(int count)
        {
            if (_signal.CurrentCount + count > _maxWorkerCount || _signal.CurrentCount + count < 0)
                throw new InvalidWokerCountException($"Max worker count can't be larger than {_maxWorkerCount.ToString()} or less than 0.");
            else
            {
                if (count > 0)
                {
                    _signal.Release(count);
                }
                else
                {
                    for (var i = 0; i < count; i++)
                    {
                        _signal.WaitAsync(0);
                    }
                }
            }
        }
    }
}
