using System;
using System.Collections.Concurrent;
using System.Net.WebSockets;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;

namespace CloudService.Messaging.Middlewares.WebsocketConsoleMiddleware
{
    internal class WebSocketMessageBroadcaster
    {
        private ConcurrentDictionary<string, WebSocket> _sockets = new ConcurrentDictionary<string, WebSocket>();
        public WebSocket GetSocketById(string id)
        {
            return _sockets.FirstOrDefault(p => p.Key == id).Value;
        }

        public ConcurrentDictionary<string, WebSocket> GetAll()
        {
            return _sockets;
        }

        public string GetId(WebSocket socket)
        {
            return _sockets.FirstOrDefault(p => p.Value == socket).Key;
        }

        public void AddSocket(WebSocket socket)
        {
            _sockets.TryAdd(CreateConnectionId(), socket);
        }
        public async Task RemoveSocket(string id)
        {
            if (string.IsNullOrEmpty(id)) return;

            WebSocket socket;
            _sockets.TryRemove(id, out socket);

            if (socket.State != WebSocketState.Open) return;

            await socket.CloseAsync(closeStatus: WebSocketCloseStatus.NormalClosure,
                                    statusDescription: "Closed by the WebSocketMessageBroadcaster",
                                    cancellationToken: CancellationToken.None).ConfigureAwait(false);
        }

        public async Task RemoveSocket(WebSocket socket)
        {
            if (socket == null) return;
            var id = this.GetId(socket);
            if (string.IsNullOrEmpty(id)) return;

            _sockets.TryRemove(id, out WebSocket s);

            if (s.State != WebSocketState.Open) return;

            await s.CloseAsync(closeStatus: WebSocketCloseStatus.NormalClosure,
                                    statusDescription: "Closed by the WebSocketMessageBroadcaster",
                                    cancellationToken: CancellationToken.None).ConfigureAwait(false);
        }

        private string CreateConnectionId()
        {
            return Guid.NewGuid().ToString();
        }
    }
}
