using System.Collections.Generic;
using System.Diagnostics;
using BotCore.Configuration;

namespace BotCore.Interfaces
{
    public interface IServerService
    {
        Dictionary<Server, Process> ServerProcesses { get; }

        void StartServer(Server server);
        void StopServer(Server server);
    }
}