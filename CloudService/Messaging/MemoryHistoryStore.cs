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
        public MemoryHistoryStore()
        {
            _store = new SortedList<long, IMessage>();
        }

        public void Add(IMessage message)
        {
            _store.Add(0 - message.Ticks, message);
        }

        public IEnumerable<IMessage> Last(int count)
        {
            return _store.Take(count).Select(x => x.Value).Reverse();
        }
        public IEnumerable<IMessage> Last(int count, string jobName)
        {
            return _store.Where(x => x.Value.JobName == jobName).Take(count).Select(x => x.Value).Reverse();
        }
        public IEnumerable<IMessage> Last(int count, string jobName, string workerId)
        {
            return _store.Where(x => x.Value.JobName == jobName && x.Value.JobThreadId == workerId).Take(count).Select(x => x.Value).Reverse();
        }
    }
}
