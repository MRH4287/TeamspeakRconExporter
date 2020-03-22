using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TeamSpeak3QueryApi.Net;
using TeamSpeak3QueryApi.Net.Specialized;
using TeamspeakRconExporter.Models;

namespace TeamspeakRconExporter
{
    public class Connection : IDisposable
    {
        public TeamSpeakClient Client { get; private set; }

        public Config Config { get;}

        public Connection(Config config)
        {
            Config = config;
        }

        public async Task Connect()
        {
            Client = new TeamSpeakClient(Config.Hostname, Config.Port ?? QueryClient.DefaultPort);

            await Client.Connect().ConfigureAwait(false);
            await Login().ConfigureAwait(false);
            await UseServer().ConfigureAwait(false);
        }

        public async Task Disconnect()
        {
            await Client.Logout().ConfigureAwait(false);
            Client.Dispose();
            Client = null;
        }

        public async Task Reconnect()
        {
            await Disconnect().ConfigureAwait(false);
            await Task.Delay(1000).ConfigureAwait(false);
            await Connect().ConfigureAwait(false);
        }

        public Task Login()
        {
            return Client.Login(Config.Username, Config.Password);
        }

        public Task UseServer()
        {
            return Client.UseServer(Config.ServerId);
        }

        public void Dispose()
        {
            ((IDisposable)Client).Dispose();
        }
    }
}
