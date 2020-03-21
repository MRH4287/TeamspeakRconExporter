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
        private readonly Gauge _clients = Metrics.CreateGauge(Consts.GetMetricName("clients"), "The currently connected Clients", "NickName");

        #endregion

        private readonly Connection _connection;
        private List<string> _lastSeenClients = new List<string>();


        public ClientsCollector(Connection connection)
        {
            _connection = connection;
        }

        public async Task Collect()
        {
            var allClients = await _connection.GetClients().ConfigureAwait(false);
            var clients = allClients.Where(c => c.Type == TeamSpeak3QueryApi.Net.Specialized.ClientType.FullClient).ToList();

            _clientCount.Set(clients.Count);

            var clientHash = new HashSet<string>();
            foreach (var client in clients)
            {
                _clients.WithLabels(client.NickName).Set(1);
                clientHash.Add(client.NickName);
            }

            foreach (var lastSeen in _lastSeenClients)
            {
                if (!clientHash.Contains(lastSeen))
                {
                    _clientCount.WithLabels(lastSeen).Set(0);
                }
            }

            _lastSeenClients = clientHash.ToList();

        }
    }
}
