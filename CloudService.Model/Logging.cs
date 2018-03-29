using System;
using System.Collections.Generic;
using System.Text;

namespace CloudService.Model
{
    public class Logging
    {
        public DateTime Timestamp { get; set; }
        public LogLevel Level { get; set; }
        public string Message { get; set; }
    }

    public enum LogLevel
    {
        Warning, Debug, Information, Error
    }
}
