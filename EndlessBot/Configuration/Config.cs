using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

namespace BotCore.Configuration
{
    public class Config
    {   
        [JsonProperty("Admins")]
        public static IEnumerable<string> Admins { get; set; }

        [JsonProperty("AllowedChannels")]
        public static IEnumerable<string> AllowedChannels { get; set; }

        [JsonProperty("CommandPrefix")]
        public static string CommandPrefix { get; set; }

        [JsonProperty("OwnerId")]
        public static string OwnerId { get; set; }

        [JsonProperty("Servers")]
        public static IEnumerable<Server> Servers { get; set; }

        [JsonProperty("Token")]
        public static string Token { get; set; }


        public static void LoadConfig()
        {
            Console.WriteLine("Reading configuration file.");

            try
            {
                string jsonString = File.ReadAllText("config.json");
                JsonConvert.DeserializeObject(jsonString, typeof(Config));
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                Console.WriteLine("Your config file is not presented or corrupt");
            }
        }
    }

    public class Server
    {
        public string Ip { get; set; }
        public string Port { get; set; }
        public string Name { get; set; }
        public string ByondAddress => $"byond://{Ip}:{Port}";
    }
}