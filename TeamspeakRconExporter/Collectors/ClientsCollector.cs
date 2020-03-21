using Prometheus;
using Prometheus.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeamspeakRconExporter.Collectors
{
    public class ClientsCollector : ICollector
    {
        #region Metrics

        private readonly Gauge _clientCount = Metrics.CreateGauge(Consts.GetMetricName("clientCount"), "The number of Clients currently on the Server");
        private readonly Gauge _clients = Metrics.CreateGauge(Consts.GetMetricName("clients"), "The currently connected Clients", "NickName", "ChannelName");

        #endregion

        private readonly Connection _connection;
        private List<(string, string)> _lastSeenClients = new List<(string, string)>();


        public ClientsCollector(Connection connection)
        {
            _connection = connection;
        }

        public async Task Collect()
        {
            var allClients = await _connection.GetClients().ConfigureAwait(false);
            var channels = await _connection.GetChannels().ConfigureAwait(false);
            var clients = allClients.Where(c => c.Type == TeamSpeak3QueryApi.Net.Specialized.ClientType.FullClient).ToList();
            var channelMap = channels.ToDictionary(k => k.Id);

            _clientCount.Set(clients.Count);

            var lastSeenData = new List<(string, string)>();
            foreach (var client in clients)
            {
                var channelName = channelMap.TryGetValue(client.ChannelId, out var channel) ? channel.Name : "";

                _clients.WithLabels(client.NickName, channelName).Set(1);
                lastSeenData.Add((client.NickName, channelName));
            }

            foreach (var lastSeen in _lastSeenClients)
            {
                if (!lastSeenData.Any(d => d.Item1 == lastSeen.Item1 && d.Item2 == lastSeen.Item2))
                {
                    _clients.WithLabels(lastSeen.Item1, lastSeen.Item2).Set(0);
                }
            }

            _lastSeenClients = lastSeenData;

        }
    }
}
