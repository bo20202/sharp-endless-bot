using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Net.Http;
using System.Threading.Tasks;
using EndlessConfiguration;
using EndlessConfiguration.Models;
using EndlessConfiguration.Models.Backend.Result;
using Microsoft.AspNetCore.WebUtilities;
using Newtonsoft.Json;
using Action = EndlessConfiguration.Models.Backend.Action;

namespace BotCore.Services
{
    public static class BackendRequestService
    {
        private static readonly HttpClient Client = new HttpClient();
        private static readonly string Address = Config.Backend.Address;
        public static async Task<ServerInfo> MakeStatusRequest(string ip, string port)
        {
            try
            {
                const string controllerName = "ServerStatus";
                const string actionName = "Get";

                var url = BuildUri(controllerName, actionName);
                url = QueryHelpers.AddQueryString(url, "ip", ip);
                url = QueryHelpers.AddQueryString(url, "port", port);


                var responce = await Client.GetStringAsync(url);
                ServerInfo info = JsonConvert.DeserializeObject<ServerInfo>(responce);
                return info;
            }
            catch (TimeoutException ex)
            {
                Console.WriteLine(ex.Message);
                return new ServerInfo() {Admins = 0, IsOnline = false, Players = 0};
            }
        }

        public static async Task<GameServerResponceModel> UpdateRequest(string serverName)
        {
            try
            {
                const string controllerName = "Updater";
                const string actionName = "Update";
                TimeSpan oldSpan = Client.Timeout;
                Client.Timeout = TimeSpan.MaxValue;

                var url = BuildUri(controllerName, actionName);
                var parameters = new Dictionary<string, string> {{"serverName", serverName}};
                var content = new FormUrlEncodedContent(parameters);
                var responce = await Client.PostAsync(url, content);

                return JsonConvert.DeserializeObject<GameServerResponceModel>(await responce.Content.ReadAsStringAsync());

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return new GameServerResponceModel("Fail.", true);
            }
        }


        private static string BuildUri(string controllerName, string actionName)
        {
            UriBuilder builder = new UriBuilder(Address);
            builder.Path += controllerName;
            builder.Path += actionName;

            return builder.ToString();
        }
    }
}
