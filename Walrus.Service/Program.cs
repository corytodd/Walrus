using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Walrus.Service;

IHost host = Host.CreateDefaultBuilder(args)
    .UseWindowsService(options =>
    {
        options.ServiceName = "Walrus Git Browser Service";
    })
    .ConfigureServices(services =>
    {
        services.AddHostedService<WalrusService>();
    })
    .Build();

await host.RunAsync();
