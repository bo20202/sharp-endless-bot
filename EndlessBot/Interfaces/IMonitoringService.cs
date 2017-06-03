using System;
using System.Collections.Generic;
using BotCore.Configuration;
using BotCore.Services.ServerMonitoring;
using Discord.Rest;
using Discord.WebSocket;

namespace BotCore.Interfaces
{
    public interface IMonitoringService
    {
        Dictionary<ISocketMessageChannel, bool> IsMonitoring { get; }
        Dictionary<ISocketMessageChannel, Dictionary<Server, RestUserMessage>> Messages { get; set; }
        event EventHandler<ServerMonitoringEventArgs> GotServerData;

        void StartMonitoring(ISocketMessageChannel channel);
        void StopMonitoring(ISocketMessageChannel channel);
        void InitializeForChannel(ISocketMessageChannel channel);
    }
}