using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TeamSpeak3QueryApi.Net;
using TeamSpeak3QueryApi.Net.Specialized;
using TeamspeakRconExporter.Models;

namespace TeamspeakRconExporter
{
    public class Connection : TeamSpeakClient
    {
        public Config Config { get;}

        public Connection(Config config)
            : base(config.Hostname, config.Port ?? QueryClient.DefaultPort)
        {
            Config = config;
        }

        public Task Login()
        {
            return Login(Config.Username, Config.Password);
        }

        public Task UseServer()
        {
            return UseServer(Config.ServerId);
        }
    }
}
