using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Sidekick.Common;
using Sidekick.Common.Initialization;
using Sidekick.Data.Builder;
using Sidekick.Data.Builder.Cli;

#region Services

var services = new ServiceCollection();
services.AddLogging(o =>
{
    o.SetMinimumLevel(LogLevel.Trace);
    o.AddFilter("Microsoft", LogLevel.Warning);
    o.AddFilter("System", LogLevel.Warning);
    o.AddConsole();
});

services.AddSingleton<CommandExecutor>();
services.AddSidekickCommon(SidekickApplicationType.DataBuilder);
services.AddSidekickDataBuilder();

#endregion

var serviceProvider = services.BuildServiceProvider();

var logger = serviceProvider.GetRequiredService<ILogger<Program>>();
var configuration = serviceProvider.GetRequiredService<IOptions<SidekickConfiguration>>();
var initializableServices = new List<IInitializableService>();
foreach (var serviceType in configuration.Value.InitializableServices)
{
    var service = serviceProvider.GetRequiredService(serviceType);
    if (service is not IInitializableService initializableService) continue;

    initializableServices.Add(initializableService);
}

foreach (var service in initializableServices.OrderBy(x => x.Priority))
{
    logger.LogInformation($"[Initialization] Initializing {service.GetType().FullName}");
    await service.Initialize();
}

var executor = serviceProvider.GetRequiredService<CommandExecutor>();
return await executor.Execute();
