using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using BotCore.Configuration;
using Microsoft.AspNetCore.WebUtilities;

namespace BotCore.Services.ServerMonitoring
{
    public class ServerMonitoringEventArgs : EventArgs
    {
        public ServerInfo ServerInfo;
    }

    public class ServerMonitoringService
    {
        private bool _isMonitoring;
        private readonly List<Timer> _timers;
        private readonly string _monitorCommand;

        public event EventHandler<ServerMonitoringEventArgs> GotServerData;

        public ServerMonitoringService()
        {
            _isMonitoring = false;
            _timers = new List<Timer>();
            _monitorCommand = "status";
        }

        public void StartMonitoring()
        {
            if(_isMonitoring)
                return;
            foreach (var server in Config.Servers)
            {
                var timer = new Timer(Monitor, server, 0, 10000);
                _timers.Add(timer);
            }
            _isMonitoring = true;
        }

        public void StopMonitoring()
        {
            if (!_isMonitoring)
                return;
            foreach (var timer in _timers)
            {
                timer.Dispose();
            }
            _isMonitoring = false;
        }

        private void Monitor(object serverArg)
        {
            Task.Run(async () =>
            {
                Server server = (Server)serverArg;

                var topic = new ByondTopic.ByondTopic();
                var serverData = await topic.GetData(server.Ip, server.Port, _monitorCommand);
                var info = ParseByondResponce(serverData);
                ServerMonitoringEventArgs args = new ServerMonitoringEventArgs {ServerInfo = info};
                args.ServerInfo.Server = server;
                GotServerData?.Invoke(this, args);
            });
        }

        private ServerInfo ParseByondResponce(string responce)
        {
            if (responce == null)
            {
                return new ServerInfo {IsOnline = false};
            }
            var parsedQuery = QueryHelpers.ParseQuery(responce);  
            return new ServerInfo {Admins = int.Parse(parsedQuery["admins"]), Players = int.Parse(parsedQuery["players"])};
        }
    }
}
