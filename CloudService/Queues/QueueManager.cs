using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;

namespace CloudService.Queues
{
    public interface IQueueManager
    {
        IQueue GetQueue(string name);
    }

    internal class QueueManager : IQueueManager
    {
        private Dictionary<string, IQueue> _qs;
        public QueueManager()
        {
            _qs = new Dictionary<string, IQueue>();
        }

        public IQueue GetQueue(string name)
        {
            if (_qs.ContainsKey(name.ToLower()))
            {
                return _qs[name.ToLower()];
            }
            else
            {
                var q = new MemoryQueue();
                _qs.Add(name.ToLower(), q);
                return q;
            }
        }
    }
}
