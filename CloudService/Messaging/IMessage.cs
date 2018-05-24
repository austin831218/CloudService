using System;
using NLog;

namespace CloudService.Messaging
{
    public interface IMessage
    {
        MessageType Type { get; set; }
        LogLevel Level { get; set; }
        string JobName { get; set; }
        string JobThreadId { get; set; }
        string Content { get; set; }
        long Ticks { get; set; }
        object Data { get; set; }
    }
}
