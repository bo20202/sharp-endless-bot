using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using BotCore.Configuration;
using BotCore.Interfaces;

namespace BotCore.Services.ServerMonitoring
{
    public class ServerService : IServerService
    {
        public Dictionary<Server, Process> ServerProcesses { get; }
        private readonly IMonitoringService _monitoringService;

        public ServerService(IMonitoringService service)
        {
            ServerProcesses = new Dictionary<Server, Process>();
            _monitoringService = service;
        }

        public void StartServer(Server server)
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                StartServerOnLinux(server);
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                throw new Exception("Your operating system is not supported");
            }
        }

        public void StopServer(Server server)
        {
            ServerProcesses[server].Kill();
            ServerProcesses[server].Dispose();
            ServerProcesses.Remove(server);
        }

        private void StartServerOnLinux(Server server)
        {
            if (ServerProcesses.ContainsKey(server))
            {
                return;
            }

            var serverProcess = new Process
            {
                StartInfo =
                {
                    FileName = "DreamDaemon",
                    Arguments = $"{server.ExecutablePath + server.ExecutableName} {server.Port} -safe -invisible",
                    RedirectStandardOutput = true
                }
            };
            ServerProcesses[server] = serverProcess;
            serverProcess.Start();

        }
    }
}