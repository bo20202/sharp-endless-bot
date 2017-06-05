using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using BotCore.Configuration;
using BotCore.Interfaces;

namespace BotCore.Services.ServerMonitoring
{
    public class ServerService : IServerService
    {
        public Dictionary<Server, Process> ServerProcesses { get; }
        private IMonitoringService _monitoringService;

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
                StartServerOnWindows(server);
            }
            else
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


        private void StartServerOnWindows(Server server)
        {
            throw new NotImplementedException();
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
                    UseShellExecute = false,
                    FileName = "DreamDaemon",
                    Arguments = $"{server.ExecutablePath + server.ExecutableName} {server.Port} -safe -invisible",
                    CreateNoWindow = true
                }
            };

            serverProcess.Start();
            _monitoringService.PauseMonitoring();
            ServerProcesses[server] = serverProcess;
            _monitoringService.ResumeMonitoring();

        }
    }
}