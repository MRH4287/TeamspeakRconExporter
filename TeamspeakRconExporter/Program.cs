using Prometheus.Client.AspNetCore;
using Prometheus.Client.DependencyInjection;
using TeamspeakRconExporter;
using TeamspeakRconExporter.Collectors;
using TeamspeakRconExporter.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddMetricFactory();

builder.Services.AddSingleton(provider =>
{
    var config = provider.GetRequiredService<IConfiguration>();

    return config.Get<Config>()!;
});


builder.Services.AddSingleton<Connection>();
builder.Services.AddHostedService<Collector>();

// Collectors

builder.Services.AddSingleton<ICollector, ClientsCollector>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UsePrometheusServer();
app.UseRouting();

app.UseAuthorization();

app.MapRazorPages();

app.Run();
