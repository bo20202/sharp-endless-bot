using EndlessConfiguration;
using EndlessConfiguration.Models;
using EndlessConfiguration.Models.Backend.Result;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using EndlessConfiguration.Models.Server;

namespace EndlessBackend.Services
{
    public class GameServerService
    {
        private readonly List<GameServer> _gameServers = new List<GameServer>();

        public async Task<GameServerResponceModel> StartServer(string serverIdentifier)
        {
            var server = Config.Servers.SingleOrDefault(x => x.ShortName.Equals(serverIdentifier));
            if (server == null)
            {
                return new GameServerResponceModel("Server does not exist.", true);
            }
            var gameServer = _gameServers.SingleOrDefault(x => x.Server.ShortName.Equals(server.ShortName));

            if (gameServer != null)
            {
                return await gameServer.TryStartServerProcess();
            }

            gameServer = new GameServer { Server = server };
            _gameServers.Add(gameServer);

            return await gameServer.TryStartServerProcess();

        }

        public GameServerResponceModel StopServer(string serverIdentifier)
        {
            var gameServer = _gameServers.SingleOrDefault(x => x.Server.ShortName.Equals(serverIdentifier));
            if (gameServer == null)
            {
                return new GameServerResponceModel("Server does not exist.", true);
            }
            gameServer.StopServerProcess();
            return new GameServerResponceModel($"{gameServer.Server.Name} has been stopped.", false);
        }

    }

    internal class GameServer
    {
        public StreamWriter Log { get; private set; }

        public string LogPath{get;set;}
        public Server Server { get; set; }
        public Process ServerProcess { get; set; }

        public async Task<GameServerResponceModel> TryStartServerProcess()
        {
            await Task.Yield();
            if (ServerProcess != null)
            {
                return new GameServerResponceModel("Server is already running.", true);
            }

            ServerProcess = new Process
            {
                StartInfo =
                {
                    FileName = "DreamDaemon",
                    Arguments =
                        $"{Server.ExecutablePath}/{Server.ExecutableName} {Server.Port} -safe -invisible",
                    UseShellExecute = false,
                    RedirectStandardError = true
                }
            };

            var started = ServerProcess.Start();
            if (!started)
            {
                return new GameServerResponceModel($"{Server.Name} failed to start.", true);
            }
            LogPath = $"{Config.WorkingDirectory}log_{Server.ShortName}_{DateTime.Now.ToString("yyyy_MM_dd_HH_mm_ss")}.log";

            ServerProcess.BeginErrorReadLine();
            ServerProcess.ErrorDataReceived += (sender, args) => OnStdErr(args);

            var exited = ServerProcess.WaitForExit(10000);

            if (exited)
            {
                ServerProcess.Dispose();
                return new GameServerResponceModel($"{Server.Name} has not been started. See logs for additional details.", true);
            }
            return new GameServerResponceModel($"{Server.Name} has been started.", false);
        }

        public void StopServerProcess()
        {
            try
            {
                ServerProcess.Kill();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            finally
            {
                ServerProcess.Dispose();
            }
        }

        private void OnStdErr(DataReceivedEventArgs args)
        {
            File.AppendAllText(LogPath, args.Data + '\n');
            Console.WriteLine(args.Data);
            if (args.Data == "Rebooted server.")
            {
                var newLogFile = new FileInfo(Server.ExecutablePath + $"log_{Server.ShortName}_{DateTime.Now}");
                Log = newLogFile.CreateText();
            }

        }
    }
}