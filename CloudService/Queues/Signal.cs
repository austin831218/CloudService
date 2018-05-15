using System;
using System.Collections.Generic;
using System.Text;

namespace CloudService.Queues
{
    public interface ISignal
    {
        SignalType Type { get; }
        string JobName { get; }
        string Data { get; }
    }

    internal class Signal : ISignal
    {
        public SignalType Type { get; private set; }
        public string JobName { get; private set; }
        public string Data { get; private set; }

        public Signal(SignalType type, string jobName, string data = null)
        {
            this.Type = type;
            this.JobName = jobName;
            this.Data = data;
        }
    }
}
