using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using EndlessConfiguration;
using EndlessConfiguration.Models;
using EndlessConfiguration.Models.Backend.Result;
using EndlessConfiguration.Models.Server;

namespace EndlessBackend.Services
{
    public class UpdateService
    {
        private static Server _server;

        private UpdateService(Server server)
        {
            _server = server;
        }

        public static UpdateService Inititalize (string serverName)
        {
            var server = Config.Servers.Single(x => x.ShortName == serverName);
            return server == null ? null : new UpdateService(server);
        }

        public GameServerResponceModel Update()
        {
            try
            {
                var directoryList = Directory.GetDirectories(_server.VersionsDirectory);
                if (directoryList.Length < 3)
                {
                    throw new Exception("Can't update, please initialize");
                }

                string newVersionPath = $"{_server.VersionsDirectory}{DateTime.Now:yyyyMMddHHmm}";
                string currentVersionPath = $"{_server.VersionsDirectory}{_server.CurrentVersion}";
                StartUpdateScript(newVersionPath, currentVersionPath, _server.ExecutablePath);

                string oldestVersionPath = directoryList.OrderBy(x => x).First();
                DeleteOldestVersion(oldestVersionPath);
                _server.CurrentVersion = newVersionPath;
                Config.UpdateConfig();

                return new GameServerResponceModel("Server is updated.", false);
            }
            catch (Exception e)
            {
                return new GameServerResponceModel(e.Message, true);
            }
        }

        private static void DeleteOldestVersion(string oldestVersion)
        {
            var check = new Process {StartInfo = new ProcessStartInfo("lsof", $"+D {oldestVersion}"){RedirectStandardOutput = true}};
            check.Start();
            check.WaitForExit();
            string stdout = check.StandardOutput.ReadToEnd();
            if (stdout.Contains("DreamDaem"))
            {
                throw new Exception($"Can't delete oldest folder ({oldestVersion}), build is running here. Please delete it later");
            }

            var delete = new Process {StartInfo = new ProcessStartInfo("rm", $"-rf {oldestVersion}")};
            delete.Start();
            delete.WaitForExit();
        }

        private static void StartUpdateScript(string newDirectoryPath, string previousDirectoryPath, string liveSymlinkPath)
        {
            var updateProcessStartInfo = new ProcessStartInfo("perl",
                $"update-server.pl --cdir {newDirectoryPath} --pdir {previousDirectoryPath} --lsym {liveSymlinkPath}");
            var updateProcess = new Process {StartInfo = updateProcessStartInfo};

            bool started = updateProcess.Start();

            if (!started)
            {
                throw new Exception("Failed to start update process.");
            }

            updateProcess.WaitForExit();

            if (updateProcess.ExitCode != 0)
            {
                throw new Exception($"Something went wrong. Check console.");
            }

        }
    }
}