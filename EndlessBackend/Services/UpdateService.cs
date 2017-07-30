using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using EndlessConfiguration;
using EndlessConfiguration.Models;
using EndlessConfiguration.Models.Backend.Result;

namespace EndlessBackend.Services
{
    public class UpdateService
    {
        private List<GameServerUpdateModel> Servers { get; }

        public UpdateService()
        {
            Servers = new List<GameServerUpdateModel>();
            foreach (var server in Config.Servers)
            {
                Servers.Add(new GameServerUpdateModel(server));
            }
        }

        public async Task<GameServerResponceModel> UpdateServer(string serverShortName)
        {
            var server = Servers.SingleOrDefault(x => x.Server.ShortName == serverShortName);
            if (server == null)
            {
                return new GameServerResponceModel("Server not found", true);
            }

            return await server.UpdateAsync();
        }
    }

    internal class GameServerUpdateModel
    {
        public GameServerUpdateModel(Server server)
        {
            Server = server;
        }

        public Server Server { get; set; }
        public DirectoryInfo CurrentVersion { get; set; }
        public DirectoryInfo PreviousVersion { get; set; }

        private string PreviousFolderPath => $"{Config.WorkingDirectory}cev-eris-{Server.ShortName}-previous/";
        
        private GameServerResponceModel _result = new GameServerResponceModel("Server was updated", true);

        public async Task<GameServerResponceModel> UpdateAsync()
        {
            try
            {

                await Task.Run(() =>
                {
                    if (!Directory.Exists(CurrentFolderPath) || !Directory.EnumerateFileSystemEntries(CurrentFolderPath).Any())
                    {
                        DownloadBuild();
                    }
                });

                return _result;
            }
            catch (Exception e)
            {
                _result = new GameServerResponceModel(e.Message, true);
            }
            return _result;
        
        }
        private void CreateNewVersion()
        {
            var updateProcess = new Process()
            {
                StartInfo =
                {
                    FileName = "perl",
                    WorkingDirectory = Config.WorkingDirectory,
                    Arguments = $"{Config.Updater} --cdir {CurrentFolderPath} --pdir {PreviousFolderPath} --confdir {Server.ConfigPath} --savesdir {Server.SavesPath}",
                    UseShellExecute = false
                }
            };
            updateProcess.Start();
            updateProcess.WaitForExit();
            if (updateProcess.ExitCode != 0)
            {
                _result = new GameServerResponceModel("Update process was not successful", true);
            }
        }

        private void DownloadBuild()
        {
            var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "git",
                    WorkingDirectory = Config.WorkingDirectory,
                    Arguments = $"clone {Config.ServerRepo} {CurrentFolderPath}",
                    UseShellExecute = false
                },
            };

            process.Start();
            process.WaitForExit();

            if (process.ExitCode != 0)
            {
                _result = new GameServerResponceModel("Download process was not succesfull", true);
            }
        }
    }
}