using System.Threading.Tasks;
using BotCore.Configuration;
using BotCore.Interfaces;
using BotCore.Preconditions;
using Discord.Commands;

namespace BotCore.Modules
{
    public class ServerStartStopModule : ModuleBase<SocketCommandContext>
    {
        private readonly IServerService _service;

        public ServerStartStopModule(IServerService service)
        {
            _service = service;
        }

        [Command("server")]
        [RequireAllowedRole]
        public async Task HandleCommand(string command, string lilServerName)
        {
            switch (command)
            {
                case "start":
                    await Start(lilServerName.ToLower());
                    break;
                case "stop":
                    await Stop(lilServerName.ToLower());
                    break;
            }
        }

        public async Task Start(string serverShortName)
        {
            foreach (var server in Config.Servers)
            {
                if (server.ShortName != serverShortName) continue;
                _service.StartServer(server);
                await Context.Channel.SendMessageAsync("Server started.");
                return;
            }
        }

        public async Task Stop(string serverShortName)
        {
            foreach (var server in Config.Servers)
            {
                if (server.ShortName != serverShortName) continue;
                _service.StopServer(server);
                await Context.Channel.SendMessageAsync("Server stopped");
                return;
            }
        }
    }
}