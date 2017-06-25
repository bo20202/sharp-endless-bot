using System.Collections.Generic;
using System.Diagnostics;
using BotCore.Configuration;
using Discord.Commands;

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