using System;
using System.Collections.Concurrent;
using System.Net.WebSockets;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;
using Newtonsoft.Json;
using System.Text;
using NLog;
using System.Collections.Generic;
using CloudService.Messaging.Middlewares.WebsocketConsoleMiddleware.Commands;

namespace CloudService.Messaging.Middlewares.WebsocketConsoleMiddleware
{
    internal class WebSocketMessageBroadcaster
    {
        private ILogger _logger = LogManager.GetCurrentClassLogger();
        private ConcurrentDictionary<string, WebSocket> _sockets = new ConcurrentDictionary<string, WebSocket>();
        private readonly IHistoryStore _hs;
        private readonly BroadcastMessageQueue _q;
        private readonly CancellationTokenSource _ts;
        public WebSocketMessageBroadcaster(IHistoryStore hs, BroadcastMessageQueue q)
        {
            _hs = hs;
            _q = q;
            _ts = new CancellationTokenSource();
            Task.Factory.StartNew((tk) =>
            {
                var ct = (CancellationToken)tk;
                while (!ct.IsCancellationRequested)
                {
                    var messages = _q.DequeueOrWait(ct);
                    if (messages == null || messages.Count == 0) continue;
                    var bs = messages.FindAll(x => string.IsNullOrEmpty(x.ClientId)).Select(x => x.Message).ToList();
                    this.BroadcastMessageAsync(bs).Wait();
                    messages.FindAll(x => !string.IsNullOrEmpty(x.ClientId)).ForEach(x =>
                    {
                        this.SendMessageAsync(this.GetSocketById(x.ClientId), new List<IMessage>() { x.Message }).Wait();
                    });
                }

            }, _ts.Token, _ts.Token);
        }

        public void Stop()
        {
            _ts.Cancel();
        }
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
        public void BroadcastMessage(IMessage message)
        {
            _hs.Add(message);
            _q.Add(message);
        }
        public void SendMessage(WebSocket socket, IMessage message)
        {
            _q.Add(message, this.GetId(socket)); ;
        }

        public void SendLog(WebSocket socket, Command cmd)
        {
            var logs = _hs.Last(50, cmd.JobName, cmd.WorkerId);
            logs.Reverse().ToList().ForEach(m => this.SendMessage(socket, m));

        }

        private async Task BroadcastMessageAsync(List<IMessage> messages)
        {
            foreach (var pair in GetAll())
            {
                try
                {
                    if (pair.Value.State == WebSocketState.Open)
                        await SendMessageAsync(pair.Value, messages).ConfigureAwait(false);
                }
                catch (WebSocketException e)
                {
                    if (e.WebSocketErrorCode == WebSocketError.ConnectionClosedPrematurely)
                    {
                        await RemoveSocket(pair.Value).ConfigureAwait(false);
                    }
                }
                catch (Exception ex)
                {
                    _logger.Error(ex);
                }
            }
        }

        private async Task SendMessageAsync(WebSocket socket, List<IMessage> messages)
        {
            if (socket.State != WebSocketState.Open)
                return;

            var serializedMessage = JsonConvert.SerializeObject(messages);
            var encodedMessage = Encoding.UTF8.GetBytes(serializedMessage);
            await socket.SendAsync(buffer: new ArraySegment<byte>(array: encodedMessage,
                                                                  offset: 0,
                                                                  count: encodedMessage.Length),
                                   messageType: WebSocketMessageType.Text,
                                   endOfMessage: true,
                                   cancellationToken: CancellationToken.None).ConfigureAwait(false);
        }

    }
}
