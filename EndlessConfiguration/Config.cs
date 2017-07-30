using System;
using System.Collections.Generic;
using System.IO;
using EndlessConfiguration.Models;
using EndlessConfiguration.Models.Backend;
using Newtonsoft.Json;

namespace EndlessConfiguration
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

        [JsonProperty("MainChannelId")]
        public static ulong MainChannelId { get; set; }

        [JsonProperty("MainGuildId")]
        public static ulong MainGuildId { get; set; }

        [JsonProperty("Backend")]
        public static Backend Backend { get; set; }

        [JsonProperty("SecretCode")]
        public static string SecretCode { get; set; }

        [JsonProperty("ServerRepo")]
        public static string ServerRepo { get; set; }

        [JsonProperty("WorkingDirectory")]
        public static string WorkingDirectory { get; set; }

        [JsonProperty("UpdaterScriptPath")]
        public static string Updater { get; set; }


        public static void LoadConfig(string path = "config.json")
        {
            Console.WriteLine("Reading configuration file.");

            try
            {
                string jsonString = File.ReadAllText(path);
                JsonConvert.DeserializeObject(jsonString, typeof(Config));
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                Console.WriteLine("Your config file is not presented or corrupt");
            }
        }
    }
}