using System;
using NLog;

namespace CloudService.Messaging
{
    public interface IMessage
    {
		string Name { get; set; }
		LogLevel Level { get; set; }
		string JobName { get; set; }
		string JobThreadId { get; set; }
		string Message { get; set; }
		object Data { get; set; }
    }
}
