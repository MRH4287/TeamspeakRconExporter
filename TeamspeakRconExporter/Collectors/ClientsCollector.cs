using Prometheus.Client;

namespace TeamspeakRconExporter.Collectors
{
    public class ClientsCollector(Connection connection, IMetricFactory metricFactory) : ICollector
    {
        #region Metrics


        private readonly IGauge _clientCount = metricFactory.CreateGauge(Consts.GetMetricName("clientCount"), "The number of Clients currently on the Server");
        private readonly IMetricFamily<IGauge> _clients = metricFactory.CreateGauge(Consts.GetMetricName("clients"), "The currently connected Clients", "NickName", "ChannelName");

        #endregion
        private List<(string, string)> _lastSeenClients = new List<(string, string)>();

        public async Task Collect()
        {
            if (connection.Client == null)
            {
                return;
            }

            var allClients = await connection.Client.GetClients().ConfigureAwait(false);
            var channels = await connection.Client.GetChannels().ConfigureAwait(false);
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
