using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Sidekick.Apis.Poe;
using Sidekick.Common;
using Sidekick.Data;
using Sidekick.Data.Builder;

#region Services

var services = new ServiceCollection();
services.AddLogging(o =>
{
    o.SetMinimumLevel(LogLevel.Trace);
    o.AddFilter("Microsoft", LogLevel.Warning);
    o.AddFilter("System", LogLevel.Warning);
    o.AddConsole();
});

services.AddSidekickCommon(SidekickApplicationType.DataBuilder);
services.AddSidekickPoeApi();
services.AddSidekickData();
services.AddSidekickDataBuilder();

#endregion

var serviceProvider = services.BuildServiceProvider();

try
{
    var dataBuilder = serviceProvider.GetRequiredService<DataBuilder>();
    await dataBuilder.DownloadAndBuildAll();
    return 0;
}
catch (Exception ex)
{
    var logger = serviceProvider.GetRequiredService<ILogger<Program>>();
    logger.LogCritical(ex, "Failed to execute command.");
    return 2;
}
