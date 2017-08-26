using System;
using System.Collections.Generic;
using BotCore.Services;
using Discord.Rest;
using Discord.WebSocket;
using EndlessConfiguration.Models;
using EndlessConfiguration.Models.Server;

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