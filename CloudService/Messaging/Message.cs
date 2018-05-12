using System;
using NLog;

namespace CloudService.Messaging
{
    public class Message : IMessage
    {

        public Message()
        {
        }

        public string Name { get; set; }
        public LogLevel Level { get; set; }
        public string JobName { get; set; }
        public string JobThreadId { get; set; }
        public object Data { get; set; }
        public string Content { get; set; }
    }
}
