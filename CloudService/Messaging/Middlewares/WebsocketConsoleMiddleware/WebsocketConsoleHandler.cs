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

            await Broadcaster.SendMessageAsync(socket, new Message()
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



        public async Task ReceiveAsync(WebSocket socket, WebSocketReceiveResult result, string receivedMessage)
        {
            Command cmd = JsonConvert.DeserializeObject<Command>(receivedMessage);
        }


    }
}
