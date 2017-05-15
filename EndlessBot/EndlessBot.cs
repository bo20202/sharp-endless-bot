using System;
using System.Threading.Tasks;
using BotCore.Configuration;
using BotCore.Services.ServerMonitoring;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace BotCore
{
    public class EndlessBot
    {
        private DiscordSocketClient _client;
        private CommandHandler _handler;

        public static void Main(string[] args) => new EndlessBot().Start().GetAwaiter().GetResult();

        public async Task Start()
        {
            Config.LoadConfig();
            _client = new DiscordSocketClient();
            _client.Log += Log;

            await _client.LoginAsync(TokenType.Bot, Config.Token);
            await _client.StartAsync();
            var provider = ConfigureBotServices();

            _handler = new CommandHandler(provider);
            await _handler.ConfigureAsync();
            await Task.Delay(-1);
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