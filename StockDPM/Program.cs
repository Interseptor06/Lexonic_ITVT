using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using StockDPM;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices(services => { services.AddHostedService<Worker>(); })
    .Build();

await host.RunAsync();
