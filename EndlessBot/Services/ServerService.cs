using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using BotCore.Interfaces;
using Discord.Commands;
using EndlessConfiguration;
using EndlessConfiguration.Models;

namespace BotCore.Services
{
    public class ServerService : IServerService
    {
        public Dictionary<Server, Process> ServerProcesses { get; }
        public SocketCommandContext Context { get; set; }
        private readonly Dictionary<Server, StringBuilder> _logBuilders;
        private Dictionary<Server, Timer> _timers;
        public ServerService()
        {
            ServerProcesses = new Dictionary<Server, Process>();
            _logBuilders = new Dictionary<Server, StringBuilder>();
            _timers = new Dictionary<Server, Timer>();
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
            _logBuilders.Remove(server);
            _timers[server].Dispose();
            _timers.Remove(server);
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
            serverProcess.Start();

            if (!_logBuilders.ContainsKey(server))
            {
                _logBuilders[server] = new StringBuilder(0, 2000);
            }
            serverProcess.BeginErrorReadLine();
            serverProcess.ErrorDataReceived += async (sender, args) => await OnStdErr(args, server);
            StartLogSendingTimer(server);
        }

        private void StartLogSendingTimer(Server server)
        {
            if (_timers.ContainsKey(server))
            {
                return;
            }

            _timers[server] = new Timer(async state => await SendLog(server), server, 0, 5000);

        }

        private async Task SendLog(Server server)
        {
            if(string.IsNullOrWhiteSpace(_logBuilders[server].ToString())) return;
            var channel = Context?.Guild?.GetTextChannel(server.LogChannel);
            var sendMessageAsync = channel?.SendMessageAsync(_logBuilders[server].ToString());
            if (sendMessageAsync != null)
                await sendMessageAsync;
            _logBuilders[server].Clear();
        }

        private async Task OnStdErr(DataReceivedEventArgs args, Server server)
        { 
            try
            { 
                _logBuilders[server].AppendLine(args.Data);
            }
            catch (ArgumentOutOfRangeException)
            {
                await Task.Delay(5000);
                await SendLog(server);
                _logBuilders[server].AppendLine(args.Data);
            }

        }


        
    }
}