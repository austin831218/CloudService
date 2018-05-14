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
        void Enqueue(string item, int number);
        string Dequeue();
        int Length { get; }
    }

    internal class MemoryQueue : IQueue
    {
        private ConcurrentQueue<string> _q;
        public int Length => _q.Count;
        public MemoryQueue()
        {
            _q = new ConcurrentQueue<string>();
        }

        public string Dequeue()
        {
            if (_q.TryDequeue(out string item))
            {
                return item;
            }
            return null;
        }

        public void Enqueue(string item, int number)
        {
            for (var i = 0; i < number; i++)
            {
                _q.Enqueue(item);
            }

        }
    }
}
