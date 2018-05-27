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
using CloudService.Infrastructure;

namespace CloudService.Messaging.Middlewares.WebsocketConsoleMiddleware
{
    internal class WebsocketConsoleHandler
    {
        public WebSocketMessageBroadcaster Broadcaster { get; private set; }
        private IJobWorkerManager _jobManager;

        public WebsocketConsoleHandler(WebSocketMessageBroadcaster broadcaster, IJobWorkerManager jobManager)
        {
            Broadcaster = broadcaster;
            _jobManager = jobManager;
        }

        public void OnConnected(WebSocket socket)
        {
            Broadcaster.AddSocket(socket);

            Broadcaster.SendMessage(socket, new Message()
            {
                Type = MessageType.Connected,
                Level = NLog.LogLevel.Info,
                Content = Broadcaster.GetId(socket)
            });
        }

        public virtual async Task OnDisconnected(WebSocket socket)
        {
            await Broadcaster.RemoveSocket(socket).ConfigureAwait(false);
        }



        public async Task ReceiveAsync(WebSocket socket, WebSocketReceiveResult result, string receivedMessage)
        {
            Command cmd = JsonConvert.DeserializeObject<Command>(receivedMessage);

            switch (cmd.Type)
            {
                case CommandType.ChangeCapacity:
                    _jobManager.ChangeCapacity(cmd.Count);
                    break;
                case CommandType.GetLog:
                    Broadcaster.SendLog(socket, cmd);
                    break;
                case CommandType.StopWorker:
                    _jobManager.StopWorker(cmd.WorkerId);
                    break;
                case CommandType.ChangeJobThread:
                    _jobManager.ChangeJobThreads(cmd.JobName, cmd.Count);
                    break;
            }
        }


    }
}
