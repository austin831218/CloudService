using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;

namespace CloudService.Messaging
{
    public class MemoryHistoryStore : IHistoryStore
    {
        private SortedList<long, IMessage> _store;
        private readonly object _locker;
        private long _indexer = long.MaxValue;
        public MemoryHistoryStore()
        {
            _locker = new object();
            _store = new SortedList<long, IMessage>();
        }

        public void Add(IMessage message)
        {
            lock (_locker)
            {
                _store.Add(_indexer--, message);
            }
        }

        public IEnumerable<IMessage> Last(int count)
        {
            return _store.Take(count).Select(x => x.Value).Reverse();
        }
        public IEnumerable<IMessage> Last(int count, string jobName)
        {
            if (string.IsNullOrEmpty(jobName))
            {
                return this.Last(count);
            }
            return _store.Where(x => x.Value.JobName == jobName).Take(count).Select(x => x.Value).Reverse();
        }
        public IEnumerable<IMessage> Last(int count, string jobName, string workerId)
        {
            if (string.IsNullOrEmpty(workerId))
            {
                return this.Last(count, jobName);
            }
            return _store.Where(x => x.Value.JobName == jobName && x.Value.JobThreadId == workerId).Take(count).Select(x => x.Value).Reverse();
        }
    }
}
