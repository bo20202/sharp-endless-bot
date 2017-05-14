using System;
using System.Threading.Tasks;
using BotCore.Configuration;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace BotCore
{
    public class EndlessBot
    {
        private readonly DiscordSocketClient _client;
        private CommandHandler _handler;

        public EndlessBot()
        {
            Config.LoadConfig();
            _client = new DiscordSocketClient();
            _client.Log += Log;
            _client.Connected += () =>
            {
                Console.WriteLine($"Connected with {_client.CurrentUser.Username}");
                return Task.CompletedTask;
            };

        }

        public async void Start()
        {
            
            await _client.LoginAsync(TokenType.Bot, Config.Token);
            await _client.StartAsync();
            var provider = ConfigureBotServices();

            _handler = new CommandHandler(provider);
            await _handler.ConfigureAsync();
        }


        private IServiceProvider ConfigureBotServices()
        {
            var services = new ServiceCollection()
                .AddSingleton(_client)
                .AddSingleton(new CommandService(
                    new CommandServiceConfig {CaseSensitiveCommands = false, ThrowOnError = false}));

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