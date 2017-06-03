using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using BotCore.Configuration;
using BotCore.Interfaces;

namespace BotCore.Services.ServerMonitoring
{
    public class ServerService : IServerService
    {
        public Dictionary<Server, Process> ServerProcesses { get; }

        public ServerService()
        {
            ServerProcesses = new Dictionary<Server, Process>();
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
            ServerProcesses[server]?.Kill();
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
                    UseShellExecute = true,
                    FileName = "DreamDaemon",
                    Arguments = $"{server.ExecutableName} {server.Port} -cd {server.ExecutablePath} -safe -invisible",
                    CreateNoWindow = true
                }
            };

            serverProcess.Start();
            ServerProcesses[server] = serverProcess;
        }
    }
}