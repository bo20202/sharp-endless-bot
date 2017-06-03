using System;
using System.Diagnostics;
using BotCore.Configuration;

namespace TestServerStart
{
    class Program
    {
        private static Process _process;

        static void Main(string[] args)
        {
            var server = new Server()
            {
                Ip = "123",
                Name = "as",
                Port = "2000",
                ServerPath = "/home/case/cev-eris/cev_eris.dmb",
                ShortName = "test"
            };

            Console.WriteLine("Server start/stop test");

            while (true)
            {
                string msg = Console.ReadLine();
                if (msg == "start")
                {
                    StartServerOnLinux(server);
                }
                else if(msg == "stop")
                {
                    _process.Kill();
                    Console.WriteLine("Stopped");
                }
            }
        }


        private static void StartServerOnLinux(Server server)
        {

            var serverProcess = new Process
            {
                StartInfo =
                {
                    UseShellExecute = true,
                    FileName = @"/bin/bash",
                    Arguments = $"-c cd {server.ServerPath} && DreamDaemon cev_eris.dmb {server.Port} -safe -invisible",
                    CreateNoWindow = true,
                    RedirectStandardError = true,
                    RedirectStandardOutput = true
                }
            };

            serverProcess.Start();
            _process = serverProcess;
        }
    }
}