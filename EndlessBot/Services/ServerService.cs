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
using Discord;
using Discord.Commands;
using Discord.WebSocket;

namespace BotCore.Services.ServerMonitoring
{
    public class ServerService : IServerService
    {
        public Dictionary<Server, Process> ServerProcesses { get; }
        public SocketCommandContext Context { get; set; }
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
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false
                }
            };
            ServerProcesses[server] = serverProcess;
            

            serverProcess.ErrorDataReceived += async (s, e) => await OnErrorReceived(s, e, server);
            serverProcess.OutputDataReceived += async (s, e) => await OnDataReceived(s, e, server);
            serverProcess.Exited += async (s, e) => await OnExit(s, e, server);

            serverProcess.Start();

            serverProcess.BeginErrorReadLine();
            serverProcess.BeginOutputReadLine();

        }


        private async Task OnExit(object sender, EventArgs e, Server server)
        {
            if (Context == null)
            {
                Console.WriteLine("C O N T E X T");
            }
            else if (Context.Guild == null)
            {
                Console.WriteLine("G U I L D");
            }
            ISocketMessageChannel channel = Context?.Guild?.GetTextChannel(server.LogChannel);
            var sendMessageAsync = channel?.SendMessageAsync($"{server.Name} process exited");
            if (sendMessageAsync != null)
                await sendMessageAsync;
        }

        private async Task OnDataReceived(object sender, DataReceivedEventArgs e, Server server)
        {
            if (Context == null)
            {
                Console.WriteLine("C O N T E X T");
            }
            else if (Context.Guild == null)
            {
                Console.WriteLine("G U I L D");
            }
            ISocketMessageChannel channel = Context?.Guild?.GetTextChannel(server.LogChannel);
            if (!string.IsNullOrWhiteSpace(e.Data))
            {
                var sendMessageAsync = channel?.SendMessageAsync($"INFO: {e.Data}");
                if (sendMessageAsync != null)
                    await sendMessageAsync;
            }
        }

        private async Task OnErrorReceived(object sender, DataReceivedEventArgs e, Server server)
        {
            if (Context == null)
            {
                Console.WriteLine("C O N T E X T");
            }
            else if (Context.Guild == null)
            {
                Console.WriteLine("G U I L D");
            }
            ISocketMessageChannel channel = Context?.Guild?.GetTextChannel(server.LogChannel);
            if (!string.IsNullOrWhiteSpace(e.Data))
            {
                var sendMessageAsync = channel?.SendMessageAsync($"INFO: {e.Data}");
                if (sendMessageAsync != null)
                    await sendMessageAsync;
            }
        }
    }
}