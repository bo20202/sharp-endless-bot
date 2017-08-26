using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BotCore.Interfaces;
using Discord.Rest;
using Discord.WebSocket;
using EndlessConfiguration;
using EndlessConfiguration.Models;
using EndlessConfiguration.Models.Server;

namespace BotCore.Services
{
    public class ServerMonitoringEventArgs : EventArgs
    {
        public ServerInfo ServerInfo;
    }

    public class ServerMonitoringService : IMonitoringService
    {

        public Dictionary<ISocketMessageChannel, bool> IsMonitoring { get; }
        public Dictionary<ISocketMessageChannel, Dictionary<Server, RestUserMessage>> Messages { get; set; }

        public event EventHandler<ServerMonitoringEventArgs> GotServerData;

        public ServerMonitoringService()
        {
            IsMonitoring = new Dictionary<ISocketMessageChannel, bool>();
            Messages = new Dictionary <ISocketMessageChannel, Dictionary<Server, RestUserMessage>>();
        }

        public async void StartMonitoring(ISocketMessageChannel channel)
        {
            if(IsMonitoring[channel])
                return;
            IsMonitoring[channel] = true;
            await Monitor(channel);
        }

        public void StopMonitoring(ISocketMessageChannel channel)
        {
            if (!IsMonitoring[channel])
                return;
            IsMonitoring[channel] = false;
        }

        private async Task Monitor(ISocketMessageChannel channel)
        {
            while (IsMonitoring[channel])
            {
                foreach (var server in Config.Servers)
                {
                    ServerInfo info = await BackendRequestService.MakeStatusRequest(server.Ip, server.Port);
                    info.Server = server;
                    ServerMonitoringEventArgs args = new ServerMonitoringEventArgs { ServerInfo = info };

                    args.ServerInfo.Server = server;
                    GotServerData?.Invoke(this, args);
                    await Task.Delay(5000);
                }
            }
        }

        public void InitializeForChannel(ISocketMessageChannel channel)
        {
            if (!IsMonitoring.ContainsKey(channel))
                IsMonitoring[channel] = false;
            if (!Messages.ContainsKey(channel))
                Messages[channel] = new Dictionary<Server, RestUserMessage>();
        }
    }
}
