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

                await Task.Delay(5000, _tokenSource.Token).ConfigureAwait(false);
            }
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            await _connection.Connect().ConfigureAwait(false);
            await _connection.Login().ConfigureAwait(false);
            await _connection.UseServer().ConfigureAwait(false);
            _collectorTask = Task.Run(Collect, _tokenSource.Token);
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            _tokenSource.Cancel();
            if (_collectorTask != null)
            {
                await _collectorTask.ConfigureAwait(false);
            }

            await _connection.Logout().ConfigureAwait(false);
            _connection.Dispose();
        }

        public void Dispose()
        {
            ((IDisposable)_tokenSource).Dispose();
        }
    }
}
