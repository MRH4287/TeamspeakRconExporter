using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TeamSpeak3QueryApi.Net.Specialized;

namespace TeamspeakRconExporter
{
    class Collector : IHostedService, IDisposable
    {
        private readonly Connection _connection;
        private readonly IEnumerable<ICollector> _collectors;
        private readonly ILogger<Collector> _logger;
        private CancellationTokenSource _tokenSource = new CancellationTokenSource();
        private Task _collectorTask;
        private DateTime _connectionTime;

        private TimeSpan _reconnectAfter = TimeSpan.FromMinutes(5);

        public Collector(Connection connection, IEnumerable<ICollector> collectors, ILogger<Collector> logger)
        {
            _connection = connection;
            _collectors = collectors;
            _logger = logger;
        }

        private async Task Collect()
        {
            while (!_tokenSource.IsCancellationRequested)
            {
                foreach (var collector in _collectors)
                {
                    try
                    {
                        await collector.Collect().ConfigureAwait(false);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, $"Error while executing collector '{collector.GetType().Name}'");
                    }
                }

                if ((DateTime.Now - _connectionTime) > _reconnectAfter)
                {
                    await _connection.Reconnect().ConfigureAwait(false);
                    _connectionTime = DateTime.Now;
                }

                await Task.Delay(5000, _tokenSource.Token).ConfigureAwait(false);
            }
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            await _connection.Connect().ConfigureAwait(false);
            _connectionTime = DateTime.Now;
            _collectorTask = Task.Run(Collect, _tokenSource.Token);
        }



        public async Task StopAsync(CancellationToken cancellationToken)
        {
            _tokenSource.Cancel();
            if (_collectorTask != null)
            {
                await _collectorTask.ConfigureAwait(false);
            }

            await _connection.Disconnect().ConfigureAwait(false);
            _connection.Dispose();
        }

        public void Dispose()
        {
            ((IDisposable)_tokenSource).Dispose();
        }
    }
}
