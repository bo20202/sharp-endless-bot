using System.Collections.Generic;
using System.Diagnostics;
using Discord.Commands;
using EndlessConfiguration.Models;

namespace BotCore.Interfaces
{
    public interface IServerService
    {
        Dictionary<Server, Process> ServerProcesses { get; }
        SocketCommandContext Context { get; set; }

        void StartServer(Server server);
        void StopServer(Server server);
    }
}