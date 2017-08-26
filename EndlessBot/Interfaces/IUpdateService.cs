using EndlessConfiguration;
using EndlessConfiguration.Models;
using EndlessConfiguration.Models.Server;

namespace BotCore.Interfaces
{
    public interface IUpdateService
    {
        void Update(Server server);
    }
}