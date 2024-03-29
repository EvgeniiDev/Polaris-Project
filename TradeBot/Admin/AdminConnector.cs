﻿using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SuperSocket.WebSocket.Server;
using System.Collections.Generic;

namespace TradeBot.Admin
{
    class AdminConnector
    {
        private static string GetMessage(string message)
        {
            return "blblblblblblblblblblbllblblblblblblblblb";
        }
        public static async void Server()
        {
            var host = WebSocketHostBuilder.Create()
                .UseWebSocketMessageHandler(
                    async (session, message) =>
                    {
                        await session.SendAsync(GetMessage(message.Message));
                    }
                )
                .ConfigureAppConfiguration((hostCtx, configApp) =>
                {
                    configApp.AddInMemoryCollection(new Dictionary<string, string>
                    {
                        { "serverOptions:name", "TestServer" },
                        { "serverOptions:listeners:0:ip", "127.0.0.1" },
                        { "serverOptions:listeners:0:port", "8080" }
                    });
                })
                .ConfigureLogging((hostCtx, loggingBuilder) =>
                {
                    loggingBuilder.AddConsole();
                })
                .Build();

            await host.RunAsync();
        }
    }
}
