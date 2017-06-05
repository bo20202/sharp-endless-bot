using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using BotCore.Configuration;
using BotCore.Interfaces;
using Byond;
using Discord.Rest;
using Discord.WebSocket;
using Microsoft.AspNetCore.WebUtilities;

namespace BotCore.Services.ServerMonitoring
{
    public class ServerMonitoringEventArgs : EventArgs
    {
        public ServerInfo ServerInfo;
    }

    public class ServerMonitoringService : IMonitoringService
    {
        private readonly List<Timer> _timers;
        private readonly string _monitorCommand;

        public Dictionary<ISocketMessageChannel, bool> IsMonitoring { get; }
        public Dictionary<ISocketMessageChannel, Dictionary<Server, RestUserMessage>> Messages { get; set; }

        public event EventHandler<ServerMonitoringEventArgs> GotServerData;

        public ServerMonitoringService()
        {
            IsMonitoring = new Dictionary<ISocketMessageChannel, bool>();
            _timers = new List<Timer>();
            _monitorCommand = "status";
            Messages = new Dictionary <ISocketMessageChannel, Dictionary<Server, RestUserMessage>>();
        }

        public void StartMonitoring(ISocketMessageChannel channel)
        {
            if(IsMonitoring[channel])
                return;
            foreach (var server in Config.Servers)
            {
                var timer = new Timer(Monitor, server, 0, 10000);
                _timers.Add(timer);
            }
            IsMonitoring[channel] = true;
        }

        public void StopMonitoring(ISocketMessageChannel channel)
        {
            if (!IsMonitoring[channel])
                return;
            foreach (var timer in _timers)
            {
                timer.Dispose();
            }
            IsMonitoring[channel] = false;
        }

        private void Monitor(object serverArg)
        {
            Server server = (Server) serverArg;

            var topic = new ByondTopic();
            var serverData = topic.GetData(server.Ip, server.Port, _monitorCommand);
            var info = ParseByondResponce(serverData);
            ServerMonitoringEventArgs args = new ServerMonitoringEventArgs {ServerInfo = info};

            args.ServerInfo.Server = server;
            GotServerData?.Invoke(this, args);
        }

        private ServerInfo ParseByondResponce(string responce)
        {
            if (responce == null)
            {
                return new ServerInfo {IsOnline = false};
            }
            var parsedQuery = QueryHelpers.ParseQuery(responce);  
            return new ServerInfo {Admins = int.Parse(parsedQuery["admins"]), Players = int.Parse(parsedQuery["players"]), IsOnline = true};
        }

        public void InitializeForChannel(ISocketMessageChannel channel)
        {
            if (!IsMonitoring.ContainsKey(channel))
                IsMonitoring[channel] = false;
            if (!Messages.ContainsKey(channel))
                Messages[channel] = new Dictionary<Server, RestUserMessage>();
        }

        public void PauseMonitoring()
        {
            _timers.ForEach(x => x.Change(Timeout.Infinite, Timeout.Infinite));
        }

        public void ResumeMonitoring()
        {
            _timers.ForEach(x => x.Change(0, 10000));
        }
    }
}
