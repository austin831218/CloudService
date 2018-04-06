using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.Concurrent;
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
        private SemaphoreSlim _signal;
        private CancellationTokenSource _cancelSource;
        private Task _t;
        private bool _running = false;
        private string _taskName = string.Empty;
        private int _interval = 100;

        public DateTime LastPerformedTime { get; private set; }
        public ConcurrentDictionary<int, Worker> Workers { get; private set; }
        //public ConcurrentBag<T> Errors { get; private set; }
        public string Name
        {
            get => _taskName;
            private set => _taskName = value;
        }
        //protected WorkTaskBase()
        //{
        //    Workers = new ConcurrentDictionary<int, Worker>();
        //    _cancelSource = new CancellationTokenSource();
        //    _signal = new SemaphoreSlim(1, _maxWorkerCount);
        //}

        protected WorkTaskBase(string name)
        {
            Name = name;
            Workers = new ConcurrentDictionary<int, Worker>();
            _cancelSource = new CancellationTokenSource();
            _signal = new SemaphoreSlim(1, _maxWorkerCount);
        }
        protected WorkTaskBase(WorkTaskSettings settings)
        {
            Workers = new ConcurrentDictionary<int, Worker>();
            _interval = settings.Interval;
            _cancelSource = new CancellationTokenSource();
            _signal = new SemaphoreSlim(settings.WorkerCount, _maxWorkerCount);
        }
        protected WorkTaskBase(string name, WorkTaskSettings settings)
        {
            Name = name;
            _interval = settings.Interval;
            Workers = new ConcurrentDictionary<int, Worker>();
            _cancelSource = new CancellationTokenSource();
            _signal = new SemaphoreSlim(settings.WorkerCount, _maxWorkerCount);
        }

        public void Start()
        {
            _running = true;
            Run();
        }
        public void Start(Action onStart)
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
                                        //Errors.Add(task);
                                        OnException(task, ex);
                                    }
                                    finally
                                    {
                                        LastPerformedTime = DateTime.Now;
                                        _signal.Release();
                                        Workers.TryRemove(Task.CurrentId.Value, out Worker w);
                                    }
                                });

                                Workers.TryAdd(t.Id, new Worker(t));
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
