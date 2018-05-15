using CloudService.Job;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace CloudService.Queues
{
    public interface IQueue
    {
        void Enqueue(ISignal item, int number);
        ISignal DequeueOrWait(int ms);
        int Count { get; }
    }

    internal class MemoryQueue : IQueue
    {
        private BlockingCollection<ISignal> _q;
        public int Count => _q.Count;
        public MemoryQueue()
        {
            _q = new BlockingCollection<ISignal>(new ConcurrentQueue<ISignal>());
        }

        public ISignal DequeueOrWait(int ms)
        {
            if (_q.TryTake(out ISignal item, ms))
            {
                return item;
            }
            return null;
        }

        public void Enqueue(ISignal item, int number)
        {
            for (var i = 0; i < number; i++)
            {
                _q.Add(item);
            }

        }
    }
}
