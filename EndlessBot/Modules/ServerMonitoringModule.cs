using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BotCore.Configuration;
using BotCore.Preconditions;
using BotCore.Services.ServerMonitoring;
using Discord;
using Discord.Commands;
using Discord.Rest;
using Discord.WebSocket;

namespace BotCore.Modules
{
    public class ServerMonitoringModule : ModuleBase<SocketCommandContext>
    {
        private readonly ServerMonitoringService _service;
        private readonly Dictionary<Server, RestUserMessage> _messages;

        public ServerMonitoringModule(ServerMonitoringService service)
        {
            _service = service;
            _messages = new Dictionary<Server, RestUserMessage>();
        }
        
        [RequireUserPermission(GuildPermission.Administrator)]
        [Command("start")]
        public async Task Start()
        { 
            await Context.Message.DeleteAsync();
            if (_messages.Count == 0)
            {
                foreach (var server in Config.Servers)
                {
                    _messages[server] = await Context.Channel.SendMessageAsync($"{server.Name} info is loading...");
                }
            }
            _service.StartMonitoring();
            _service.GotServerData += GotDataHandler;
        }

        [RequireUserPermission(GuildPermission.Administrator)]
        [Command("stop")]
        public async Task Stop()
        {
            await Context.Message.DeleteAsync();
            _service.StopMonitoring();
            _service.GotServerData -= GotDataHandler;
        }

        private void GotDataHandler(object sender, ServerMonitoringEventArgs args)
        {
            Task.Run(async () =>
            {
                var embed = BuildEmbed(args.ServerInfo);
                await _messages[args.ServerInfo.Server].ModifyAsync(properties => { properties.Embed = embed;
                    properties.Content = "";
                });

            });
        }

        private Embed BuildEmbed(ServerInfo info)
        {
            if (!info.IsOnline)
            {
                var embed = new EmbedBuilder().WithTitle("Server info");
                embed.AddField("Server status:", $"{info.Server.Name} is offline");
                return embed;
            }
            else
            {
                var embed = new EmbedBuilder().WithTitle("Server info");
                embed.AddField("Server status:", $"{info.Server.Name} is online.");
                embed.AddField("Server address:", info.Server.ByondAddress);
                embed.AddField("Admins:", info.Admins);
                embed.AddField("Players:", info.Players);

                return embed;
            }
        }
    }
}