using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Threading;

namespace CloudService.Messaging.Middlewares.WebsocketConsoleMiddleware
{
    internal class BroadcastMessageQueue
    {
        private readonly ConcurrentQueue<BroadcastMessage> _q;

        public BroadcastMessageQueue()
        {
            _q = new ConcurrentQueue<BroadcastMessage>();
        }

        public void Add(IMessage message, string id = null)
        {
            _q.Enqueue(new BroadcastMessage
            {
                Message = message,
                ClientId = id
            });
        }

        public List<BroadcastMessage> DequeueOrWait(CancellationToken t)
        {
            var result = new List<BroadcastMessage>();
            while (_q.IsEmpty && !t.IsCancellationRequested)
            {
                Thread.Sleep(100); // wait for element in Q
            }
            while (_q.TryDequeue(out BroadcastMessage m)) // then comsume'em all
            {
                if (m != null) result.Add(m);
            }
            return result;
        }
    }

    internal class BroadcastMessage
    {
        public string ClientId { get; set; }
        public IMessage Message { get; set; }
    }
}
