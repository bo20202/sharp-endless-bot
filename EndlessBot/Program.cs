using System;
using System.IO;
using System.Threading.Tasks;
using BotCore.Configuration;
using Newtonsoft.Json;

namespace BotCore
{
    public class Program
    {

        public static void Main(string[] args)
        {
            
            MainAsync().GetAwaiter().GetResult();
        }

        private static async Task MainAsync()
        {
            EndlessBot bot = new EndlessBot();
            bot.Start();
            await Task.Delay(-1);
        }
    }
}