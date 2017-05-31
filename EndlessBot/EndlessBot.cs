using System;
using System.Linq;
using System.Threading.Tasks;
using BotCore.Configuration;
using BotCore.Services.ServerMonitoring;
using Discord;
using Discord.Commands;
using Discord.Net;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace BotCore
{
    public class EndlessBot
    {
        private DiscordSocketClient _client;
        private CommandHandler _handler;
        private IServiceProvider _provider;

        public static void Main(string[] args) => new EndlessBot().Start().GetAwaiter().GetResult();

        public async Task Start()
        {
            try
            {
                Config.LoadConfig();
                _client = new DiscordSocketClient();
                _client.Log += Log;

                await _client.LoginAsync(TokenType.Bot, Config.Token);
                await _client.StartAsync();
                _provider = ConfigureBotServices();

                _handler = new CommandHandler(_provider);
                await _handler.ConfigureAsync();
                await Task.Delay(-1);
            }
            catch (WebSocketClosedException)
            {
                await SendRestartMeAsync();
            }
        }

        private async Task SendRestartMeAsync()
        {
            var service = _provider.GetService<ServerMonitoringService>();
            if (service.IsMonitoring.ContainsValue(true))
            {
                var channel = _client.GetGuild(Config.MainGuildId).GetTextChannel(Config.MainChannelId);
                await channel.SendMessageAsync("Somehow I was disconnected from Discord. Please restart monitoring.");
            }
        }

        private IServiceProvider ConfigureBotServices()
        {
            var services = new ServiceCollection()
                .AddSingleton(_client)
                .AddSingleton(new CommandService(
                    new CommandServiceConfig {CaseSensitiveCommands = false, ThrowOnError = false}))
                .AddSingleton<ServerMonitoringService>();

            var provider = services.BuildServiceProvider();
            return provider;
        }
        private static Task Log(LogMessage msg)
        {
            Console.WriteLine(msg.ToString());
            return Task.CompletedTask;
        }
    }
}