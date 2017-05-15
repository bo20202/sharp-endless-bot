using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using BotCore.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Discord.Commands;
using Discord.WebSocket;

namespace BotCore
{
    public class CommandHandler
    {
        private readonly IServiceProvider _provider;
        private readonly CommandService _commands;
        private readonly DiscordSocketClient _client;

        public CommandHandler(IServiceProvider provider)
        {
            _provider = provider;
            _client = _provider.GetService<DiscordSocketClient>();
            _client.MessageReceived += ProcessCommandAsync;
            _commands = provider.GetService<CommandService>();
        }

        public async Task ConfigureAsync()
        {
            await _commands.AddModulesAsync(Assembly.GetEntryAssembly());
        }

        private async Task ProcessCommandAsync(SocketMessage msgArg)
        {
            var message = msgArg as SocketUserMessage;
            if (message == null) return;
            int argPos = 0;

            if (!ParseTriggers(message, ref argPos)) return;

            var context = new SocketCommandContext(_client, message);
            await _commands.ExecuteAsync(context, argPos, _provider);
        }

        private bool ParseTriggers(SocketUserMessage message, ref int argPos)
        {
            return message.HasStringPrefix(Config.CommandPrefix, ref argPos) && Config.AllowedChannels.Any(id => ulong.Parse(id) == message.Channel.Id);
        }
    }
}