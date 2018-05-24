using System;
using System.Collections.Generic;

namespace CloudService.Messaging
{
    public interface IHistoryStore
    {
        void Add(IMessage message);
        IEnumerable<IMessage> Last(int count);
        IEnumerable<IMessage> Last(int count, string jobName);
        IEnumerable<IMessage> Last(int count, string jobName, string workerId);
    }
}
