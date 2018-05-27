using CloudService.Infrastructure;
using CloudService.Messaging.Middlewares.WebsocketConsoleMiddleware.Commands;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using NLog;

namespace CloudService.Messaging.Middlewares.WebsocketConsoleMiddleware
{
    internal class Middleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger _logger = LogManager.GetCurrentClassLogger();
        private WebsocketConsoleHandler _webSocketHandler { get; set; }
        public Middleware(RequestDelegate next, WebsocketConsoleHandler webSocketHandler)
        {
            _logger.Info("ws console middleware engaged");
            _next = next;
            _webSocketHandler = webSocketHandler;
        }

        public async Task Invoke(HttpContext context)
        {
            if (!context.WebSockets.IsWebSocketRequest)
            {
                _logger.Trace("not ws request, pass to next middleware");
                await _next.Invoke(context);
                return;
            }
            _logger.Trace("ws request received");
            var socket = await context.WebSockets.AcceptWebSocketAsync().ConfigureAwait(false);
            _webSocketHandler.OnConnected(socket);

            await Receive(socket, async (result, serializedMessage) =>
            {
                if (result.MessageType == WebSocketMessageType.Text)
                {
                    _logger.Trace($"ws command recived: {serializedMessage}");
                    await _webSocketHandler.ReceiveAsync(socket, result, serializedMessage).ConfigureAwait(false);
                    return;
                }
                else if (result.MessageType == WebSocketMessageType.Close)
                {
                    try
                    {
                        _logger.Trace("ws closing");
                        await _webSocketHandler.OnDisconnected(socket);
                        _logger.Trace("ws closed");
                    }
                    catch (WebSocketException wse)
                    {
                        _logger.Error(wse, "error when closing ws, exception raised");
                        throw;
                    }

                    return;
                }
            });
        }

        private async Task Receive(WebSocket socket, Action<WebSocketReceiveResult, string> handleMessage)
        {
            while (socket.State == WebSocketState.Open)
            {
                ArraySegment<Byte> buffer = new ArraySegment<byte>(new Byte[1024 * 4]);
                string message = null;
                WebSocketReceiveResult result = null;
                try
                {
                    using (var ms = new MemoryStream())
                    {
                        do
                        {
                            result = await socket.ReceiveAsync(buffer, CancellationToken.None).ConfigureAwait(false);
                            ms.Write(buffer.Array, buffer.Offset, result.Count);
                        }
                        while (!result.EndOfMessage);

                        ms.Seek(0, SeekOrigin.Begin);

                        using (var reader = new StreamReader(ms, Encoding.UTF8))
                        {
                            message = await reader.ReadToEndAsync().ConfigureAwait(false);
                        }
                    }

                    handleMessage(result, message);
                }
                catch (WebSocketException e)
                {
                    if (e.WebSocketErrorCode == WebSocketError.ConnectionClosedPrematurely)
                    {
                        socket.Abort();
                    }
                }
                catch (Exception ex)
                {
                    _logger.Fatal(ex, "un-handled exception");
                }
            }

            await _webSocketHandler.OnDisconnected(socket);
        }
    }
}
