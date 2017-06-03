using System.Threading.Tasks;
using BotCore.Configuration;
using BotCore.Interfaces;
using BotCore.Preconditions;
using Discord.Commands;

namespace BotCore.Modules
{
    public class ServerStartStopModule
    {
        private readonly IServerService _service;

        public ServerStartStopModule(IServerService service)
        {
            _service = service;
        }

        [Command("server")]
        [RequireAllowedRole]
        public async Task HandleCommand(string args)
        {
            var splatArgs = args.Split(' ');
            switch (splatArgs[1].ToLower())
            {
                case "start":
                    await Start(splatArgs[2].ToLower());
                    break;
                case "stop":
                    await Stop(splatArgs[2].ToLower());
                    break;
            }
        }

        public async Task Start(string serverShortName)
        {
            foreach (var server in Config.Servers)
            {
                if (server.ShortName != serverShortName) continue;
                _service.StartServer(server);
                return;
            }
        }

        public async Task Stop(string serverShortName)
        {
            foreach (var server in Config.Servers)
            {
                if (server.ShortName != serverShortName) continue;
                _service.StopServer(server);
                return;
            }
        }
    }
}