using System;
using System.Threading.Tasks;
using BotCore.Preconditions;
using BotCore.Services.ServerMonitoring;
using Discord.Commands;

namespace BotCore.Modules
{
    public class ServerMonitoringModule : ModuleBase<SocketCommandContext>
    {
        private readonly ServerMonitoringService _service;

        public ServerMonitoringModule(ServerMonitoringService service)
        {
            _service = service;
        }

        [Command("start")]
        public async Task Start()
        {
            await Context.Channel.SendMessageAsync("Start");
            _service.StartMonitoring();
            _service.GotServerData += GotDataHandler;
        }

        [Command("stop")]
        public async Task Stop()
        {
            await Context.Channel.SendMessageAsync("Stop");
            _service.StopMonitoring();
            _service.GotServerData -= GotDataHandler;
        }

        private void GotDataHandler(object sender, ServerMonitoringEventArgs args)
        {
            Task.Run(async () =>
            {
                await Context.Channel.SendMessageAsync("Players:" + args.ServerInfo.Players);
            });
        }
    }
}