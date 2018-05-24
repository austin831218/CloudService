using System;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using NLog;

namespace CloudService.Messaging
{
    [DataContract]
    public class Message : IMessage
    {

        public Message()
        {
        }

        [DataMember]
        [JsonConverter(typeof(StringEnumConverter))]
        public MessageType Type { get; set; }
        [DataMember]
        public LogLevel Level { get; set; }
        [DataMember]
        public string JobName { get; set; }
        [DataMember]
        public string JobThreadId { get; set; }
        [DataMember]
        public long Ticks { get; set; } = DateTime.UtcNow.Ticks; // to js date: var tickDate = new Date(ticks/10000 - 2208988800000);
        [DataMember]
        public dynamic Data { get; set; }
        [DataMember]
        public string Content { get; set; }
    }
}
