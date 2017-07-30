namespace EndlessConfiguration.Models
{
    public class Server
    {
        public string Ip { get; set; }
        public string Port { get; set; }
        public string Name { get; set; }
        public string ShortName { get; set; }
        public string ExecutablePath { get; set; }
        public string ExecutableName { get; set; }
        public ulong LogChannel { get; set; }
        public string ConfigPath { get; set; }
        public string SavesPath { get; set; }
        public string DmeName { get; set; }

        public string ByondAddress => $"byond://{Ip}:{Port}";
    }


}
