using CloudService.Job;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace CloudService.Infrastructure
{
    internal class MemoryJobQueue : IJobQueue
    {
        private ConcurrentQueue<string> _q;
        public int Length => _q.Count;
        public MemoryJobQueue()
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
