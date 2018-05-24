using CloudService.Messaging;
using CloudService.Messaging.Middlewares.WebsocketConsoleMiddleware;
using CloudService.Queues;
using NLog;
using System;
using System.Collections.Generic;
using System.Text;

namespace CloudService.Infrastructure
{
    internal interface IServiceLogger<T>
    {
        void Trace(string message, params object[] args);
        void Debug(string message, params object[] args);
        void Info(string message, params object[] args);
        void Warn(string message, params object[] args);
        void Error(Exception ex, string message, params object[] args);
        void Fatal(Exception ex, string message, params object[] args);
    }

    internal class ServiceLogger<T> : IServiceLogger<T>
    {

        private readonly ILogger _logger;
        private readonly WebSocketMessageBroadcaster _broadcaster;


        public ServiceLogger(WebSocketMessageBroadcaster broadcaster)
        {
            _broadcaster = broadcaster;
            _logger = LogManager.GetLogger(typeof(T).FullName);
        }

        private void log(LogLevel level, string message, Exception ex, params object[] args)
        {
            var msg = new Message
            {
                Data = ex,
                Content = message,
                Level = level,
                Ticks = DateTime.UtcNow.Ticks,
                Type = MessageType.ServerLog
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
