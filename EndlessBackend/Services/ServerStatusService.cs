using System.Threading.Tasks;
using Byond;
using EndlessBackend.Models;
using Microsoft.AspNetCore.WebUtilities;

namespace EndlessBackend.Services
{
    public class ServerStatusService
    {
        private const string GetStatusCommand = "status";

        public async Task<ServerInfoModel> GetInfo(ServerMonitorModel serverMonitor)
        {
            var responce = await ByondTopic.GetData(serverMonitor.Ip, serverMonitor.Port, GetStatusCommand);
            return ParseByondResponce(responce);
        }

        private ServerInfoModel ParseByondResponce(string responce)
        {
            if (responce == null)
            {
                return new ServerInfoModel {IsOnline = false};
            }
            var parsedQuery = QueryHelpers.ParseQuery(responce);
            return new ServerInfoModel { Admins = int.Parse(parsedQuery["admins"]), Players = int.Parse(parsedQuery["players"]), IsOnline = true };
        }
    }
}
