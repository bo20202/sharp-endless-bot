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
            ServerProcesses[server].Kill();
        }


        private void StartServerOnWindows(Server server)
        {
            throw new NotImplementedException();
        }

        private void StartServerOnLinux(Server server)
        {
            Task.Run(() =>
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

                serverProcess.ErrorDataReceived += (sender, e) =>
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine(e.Data);
                    Console.ResetColor();
                };

                serverProcess.OutputDataReceived += (sender, args) =>
                {
                    Console.ForegroundColor = ConsoleColor.DarkYellow;
                    Console.WriteLine(args.Data);
                    Console.ResetColor();
                };
                serverProcess.Start();
                ServerProcesses[server] = serverProcess;
            });

        }
    }
}