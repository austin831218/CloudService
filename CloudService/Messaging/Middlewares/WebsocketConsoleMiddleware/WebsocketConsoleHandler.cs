using System;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Collections.Generic;
using CloudService.Messaging.Middlewares.WebsocketConsoleMiddleware.Commands;

namespace CloudService.Messaging.Middlewares.WebsocketConsoleMiddleware
{
    internal class WebsocketConsoleHandler
    {
        public WebSocketMessageBroadcaster Broadcaster { get; private set; }

        public WebsocketConsoleHandler(WebSocketMessageBroadcaster broadcaster)
        {
            Broadcaster = broadcaster;
        }

        public async Task OnConnected(WebSocket socket)
        {
            Broadcaster.AddSocket(socket);

            await SendMessageAsync(socket, new Message()
            {
                Type = MessageType.Connected,
                Level = NLog.LogLevel.Info,
                Content = Broadcaster.GetId(socket)
            }).ConfigureAwait(false);
        }

        public virtual async Task OnDisconnected(WebSocket socket)
        {
            await Broadcaster.RemoveSocket(socket).ConfigureAwait(false);
        }

        public async Task SendMessageAsync(WebSocket socket, Message message)
        {
            if (socket.State != WebSocketState.Open)
                return;

            var serializedMessage = JsonConvert.SerializeObject(message);
            var encodedMessage = Encoding.UTF8.GetBytes(serializedMessage);
            await socket.SendAsync(buffer: new ArraySegment<byte>(array: encodedMessage,
                                                                  offset: 0,
                                                                  count: encodedMessage.Length),
                                   messageType: WebSocketMessageType.Text,
                                   endOfMessage: true,
                                   cancellationToken: CancellationToken.None).ConfigureAwait(false);
        }

        public async Task SendMessageAsync(string socketId, Message message)
        {
            await SendMessageAsync(Broadcaster.GetSocketById(socketId), message).ConfigureAwait(false);
        }

        public async Task SendMessageToAllAsync(Message message)
        {
            foreach (var pair in Broadcaster.GetAll())
            {
                try
                {
                    if (pair.Value.State == WebSocketState.Open)
                        await SendMessageAsync(pair.Value, message).ConfigureAwait(false);
                }
                catch (WebSocketException e)
                {
                    if (e.WebSocketErrorCode == WebSocketError.ConnectionClosedPrematurely)
                    {
                        await OnDisconnected(pair.Value);
                    }
                }
            }
        }

        public async Task ReceiveAsync(WebSocket socket, WebSocketReceiveResult result, string receivedMessage)
        {
            Command cmd = JsonConvert.DeserializeObject<Command>(receivedMessage);
        }


    }
}
