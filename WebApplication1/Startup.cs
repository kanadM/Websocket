using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using WebApplication1.Entities;
using WebApplication1.Handler;

namespace WebApplication1
{
    public class Startup
    {
        private static readonly MessageStore<Tuple<string, byte[], WebSocketReceiveResult>> msgQueue = new MessageStore<Tuple<string, byte[], WebSocketReceiveResult>>();
        private readonly WebsocketStore<string, WebSocket> webSockets = new WebsocketStore<string, WebSocket>();
        public void ConfigureServices(IServiceCollection services)
        {
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(LogLevel.Debug);
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseWebSockets();

            var webSocketOptions = new WebSocketOptions()
            {
                KeepAliveInterval = TimeSpan.FromSeconds(120),
                ReceiveBufferSize = 4 * 1024
            };
            app.UseWebSockets(webSocketOptions);

            app.Use(async (context, next) =>
            {
                if (context.Request.Path == "/ws")
                {
                    if (context.WebSockets.IsWebSocketRequest)
                    {
                        WebChatHandler webChatHandler = new WebChatHandler(msgQueue, webSockets);
                        await webChatHandler.EntertainNew(context);
                    }
                    else
                    {
                        context.Response.StatusCode = 400;
                    }
                }
                else
                {
                    await next();
                }
            });
            Task.Run(async () =>
            {
                await MessageStoreHandler.HandlePendingMessages(msgQueue, webSockets);
            });
            Task.Run(async () =>
            {
                await MessageStoreHandler.HandlePendingMessages(msgQueue, webSockets);
            });
            app.UseFileServer();
        }



    }
}
