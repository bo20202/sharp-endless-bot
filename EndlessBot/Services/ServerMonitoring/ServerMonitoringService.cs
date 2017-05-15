using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using BotCore.Configuration;
using Microsoft.AspNetCore.WebUtilities;

namespace BotCore.Services.ServerMonitoring
{
    public class ServerMonitoringService
    {
        private bool _isMonitoring;
        private readonly List<Timer> _timers;
        private readonly IServiceProvider _provider;
        private string _monitorCommand;

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
            foreach (var timer in _timers)
            {
                timer.Dispose();
            }
            _isMonitoring = false;
        }

        private async void Monitor(object serverArg)
        {
            try
            {
                Server server = (Server) serverArg;
                var topic = new ByondTopic.ByondTopic();
                var serverData = await topic.GetData(server.Ip, server.Port, _monitorCommand);

            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        private ServerInfo ParseByondResponce(string responce)
        {
            var parsedQuery = QueryHelpers.ParseQuery(responce);

            return new ServerInfo() {Admins = int.Parse(parsedQuery["admins"]), Players = int.Parse(parsedQuery["players"])};
        }
    }
}
