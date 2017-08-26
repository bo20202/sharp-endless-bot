using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using BotCore.Modules;
using Discord;
using Microsoft.Extensions.DependencyInjection;
using Discord.Commands;
using Discord.WebSocket;
using EndlessConfiguration;

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
            _commands = provider.GetService<CommandService>();
            _client.MessageReceived += ProcessCommandAsync;
            _commands.Log += Log;
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
            var a = message.HasStringPrefix(Config.CommandPrefix, ref argPos);
            return a;
        }

        private static Task Log(LogMessage msg)
        {
            Console.WriteLine(msg.ToString());
            return Task.CompletedTask;
        }
    }
}