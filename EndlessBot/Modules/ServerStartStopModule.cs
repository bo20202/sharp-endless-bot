﻿using System.Diagnostics;
using System.Threading.Tasks;
using BotCore.Interfaces;
using BotCore.Preconditions;
using Discord.Commands;
using EndlessConfiguration;

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
            _service.Context = Context;
            switch (command)
            {
                case "start":
                    await Start(lilServerName.ToLower());
                    break;
                case "stop":
                    await Stop(lilServerName.ToLower());
                    break;
                default:
                    return;
            }
        }

        public async Task Start(string serverShortName)
        {
            foreach (var server in Config.Servers)
            {
                if (server.ShortName != serverShortName) continue;
                _service.StartServer(server);
                await Context.Channel.SendMessageAsync("Server is starting, please wait.");
                return;
            }
        }

        public async Task Stop(string serverShortName)
        {
            foreach (var server in Config.Servers)
            {
                if (server.ShortName != serverShortName) continue;
                _service.StopServer(server);
                await Context.Channel.SendMessageAsync("Server has stopped.");
                return;
            }
        }
    }
}