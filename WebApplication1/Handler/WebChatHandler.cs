using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WebApplication1.Entities;

namespace WebApplication1.Handler
{

    public class WebChatHandler
    {

        private readonly MessageStore<Tuple<string, byte[], WebSocketReceiveResult>> msgQueue;
        private readonly WebsocketStore<string, WebSocket> webSockets;
        public WebChatHandler(MessageStore<Tuple<string, byte[], WebSocketReceiveResult>> _msgQueue, WebsocketStore<string, WebSocket> _webSockets)
        {
            msgQueue = _msgQueue;
            webSockets = _webSockets;
        }
        internal async Task EntertainNew(HttpContext context)
        {
            string userName = context.Request.Query["userName"];
            WebSocket webSocket = await context.WebSockets.AcceptWebSocketAsync();

            if (webSockets.CanAdd(userName))
                await Play(context, webSocket);
            else
                await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "This username already taken.", CancellationToken.None);
        }

        private async Task Play(HttpContext context, WebSocket webSocket)
        {

            string userName = context.Request.Query["userName"];

            webSockets.Add(userName, webSocket);

            var buffer = new byte[1024 * 4];
            WebSocketReceiveResult result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);

            while (!result.CloseStatus.HasValue)
            {
                string reqString = Encoding.UTF8.GetString(buffer);

                MessageReq msgReq = JsonConvert.DeserializeObject<MessageReq>(reqString);


                if (string.IsNullOrEmpty(msgReq.To))
                    await sendToEveryOneAsync(msgReq.GetBytes(), result);
                else
                    await sendToPersonAsync(msgReq.GetBytes(), result, msgReq.To);
                result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
            }
            await webSocket.CloseAsync(result.CloseStatus.Value, result.CloseStatusDescription, CancellationToken.None);
            webSockets.Remove(userName);
        }

        public async Task sendToEveryOneAsync(byte[] buffer, WebSocketReceiveResult result)
        {
            foreach (var webSocket in webSockets.Values)
            {
                if (webSocket.State == WebSocketState.Open)
                    await webSocket.SendAsync(new ArraySegment<byte>(buffer, 0, result.Count), result.MessageType, result.EndOfMessage, CancellationToken.None);
            }
        }

        public async Task sendToPersonAsync(byte[] buffer, WebSocketReceiveResult result, string to)
        {
            if (webSockets.Get(to, out var webSocket) && webSocket.State == WebSocketState.Open)
            {
                await webSocket.SendAsync(new ArraySegment<byte>(buffer, 0, result.Count), result.MessageType, result.EndOfMessage, CancellationToken.None);
            }
            else
            {
                msgQueue.Enqueue(new Tuple<string, byte[], WebSocketReceiveResult>(to, buffer, result));
            }
        }

    }

    public static class MessageStoreHandler 
    {
        public static async Task HandlePendingMessages(MessageStore<Tuple<string, byte[], WebSocketReceiveResult>> msgQueue, WebsocketStore<string, WebSocket> webSockets)
        {
            AutoResetEvent _taskWaitHandle = new AutoResetEvent(false);
            while (true)
            {
                if (msgQueue.Count > 0)
                {
                    var item = msgQueue.Dequeue();
                    if (webSockets.Get(item.Item1, out var webSocket) && webSocket.State == WebSocketState.Open)
                    {
                        await webSocket.SendAsync(new ArraySegment<byte>(item.Item2, 0, item.Item3.Count), item.Item3.MessageType, item.Item3.EndOfMessage, CancellationToken.None);
                    }
                    else
                    {
                        msgQueue.Enqueue(item);
                    }
                }
                else
                {
                    _taskWaitHandle.WaitOne(1000);
                }
            }
        }
    }
}
