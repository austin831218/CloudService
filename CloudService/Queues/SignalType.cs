using System;
using System.Collections.Generic;
using System.Text;

namespace CloudService.Queues
{
    public enum SignalType
    {
        JobScheduled,
        JobCompleted,
        JobError,
        DecreaseJobThread
    }
}
