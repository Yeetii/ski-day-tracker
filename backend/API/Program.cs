
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using backend;

var host = new HostBuilder()
    .ConfigureFunctionsWorkerDefaults()
    .ConfigureServices((hostContext, services) =>
    {
        services.AddSingleton<ICosmosClient>(new CosmosClient(
            hostContext.Configuration["CosmosDB:ConnectionString"],
            "Leaderboard",
            "Athletes"));
    })
    .Build();

host.Run();
