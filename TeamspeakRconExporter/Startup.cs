using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using Prometheus.Client.AspNetCore;
using TeamspeakRconExporter.Models;
using TeamspeakRconExporter.Collectors;

namespace TeamspeakRconExporter
{
    class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton(provider =>
            {
                var config = provider.GetService<IConfiguration>();

                return config.Get<Config>();
            });

            services.AddSingleton<Connection>();
            services.AddHostedService<Collector>();

            // Collectors

            services.AddSingleton<ICollector, ClientsCollector>();

        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory logger, IConfiguration configuration)
        {
            app.UsePrometheusServer();
        }
    }
}
