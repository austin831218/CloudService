using System;
using System.Collections.Generic;
using System.Text;
using CloudService.Job;
using CloudService.Messaging;
using CloudService.Messaging.Middlewares.WebsocketConsoleMiddleware;
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
        private readonly ILogger _logger;
        private readonly WebSocketMessageBroadcaster _broadcaster;
        public Guid WorkerId { get; private set; }

        public JobContext(IJobDescriber describer, Guid workerId, WebSocketMessageBroadcaster broadcaster)
        {
            _describer = describer;
            WorkerId = workerId;
            _broadcaster = broadcaster;
            _logger = LogManager.GetLogger(describer.Name);
        }

        private void log(LogLevel level, string message, Exception ex, params object[] args)
        {
            var msg = new Message
            {
                JobName = _describer.Name,
                JobThreadId = WorkerId.ToString(),
                Data = ex,
                Content = message,
                Level = level,
                Ticks = DateTime.UtcNow.Ticks,
                Type = MessageType.JobLog
            };
            _broadcaster.BroadcastMessageAsync(msg).Wait();
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
