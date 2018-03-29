using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;

namespace CloudService.Infrastructure.Exceptions
{
    public class InvalidWokerCountException: Exception
    {
        public InvalidWokerCountException() { }
        public InvalidWokerCountException(string message) : base(message) { }
        public InvalidWokerCountException(string message, Exception inner) : base(message, inner) { }
        protected InvalidWokerCountException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        { }
    }
}
