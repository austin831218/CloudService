using System;
using System.Collections.Generic;
using System.Text;
using CloudService.Job;
using CloudService.Messaging;
using CloudService.Queues;
using NLog;

namespace CloudService.Job
{
    public interface IJobContext
    {
        Guid WorkerId { get; }
        void Trace(string message, params object[] args);
        void Debug(string message, params object[] args);
        void Info(string message, params object[] args);
        void Warn(string message, params object[] args);
        void Error(Exception ex, string message, params object[] args);
        void Fatal(Exception ex, string message, params object[] args);
    }

    internal class JobContext : IJobContext
    {
        private readonly IJobDescriber _describer;
        private readonly IQueue _q;
        private readonly IHistoryStore _hs;
        private ILogger _logger;

        public Guid WorkerId { get; private set; }

        public JobContext(IJobDescriber describer, IQueue q, IHistoryStore hs, Guid workerId)
        {
            _describer = describer;
            _q = q;
            _hs = hs;
            WorkerId = workerId;
            _logger = LogManager.GetLogger(describer.Name);
        }

        private void log(LogLevel level, string message, Exception ex, params object[] args)
        {
            _logger.Log(level, ex, message, args);
        }

        public void Trace(string message, params object[] args)
        {
            this.log(LogLevel.Trace, message, null, args);
        }
        public void Debug(string message, params object[] args)
        {
            this.log(LogLevel.Debug, message, null, args);
        }
        public void Info(string message, params object[] args)
        {
            this.log(LogLevel.Info, message, null, args);
        }
        public void Warn(string message, params object[] args)
        {
            this.log(LogLevel.Warn, message, null, args);
        }
        public void Error(Exception ex, string message, params object[] args)
        {
            this.log(LogLevel.Error, message, ex, args);
        }
        public void Fatal(Exception ex, string message, params object[] args)
        {
            this.log(LogLevel.Fatal, message, ex, args);
        }
    }
}
