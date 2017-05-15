using System.Threading.Tasks;
using BotCore.Preconditions;
using BotCore.Services.ServerMonitoring;
using Discord.Commands;

namespace BotCore.Modules
{
    public class ServerMonitoringModule : ModuleBase
    {
        private readonly ServerMonitoringService _service;

        public ServerMonitoringModule(ServerMonitoringService service)
        {
            _service = service;
        }

        [Command("start")]
        [InAllowedUsers]
        public async Task Start()
        {
            
        }
    }
}