using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Sidekick.Apis.Poe;
using Sidekick.Common;
using Sidekick.Data;
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
services.AddSidekickPoeApi();
services.AddSidekickData();
services.AddSidekickDataBuilder();

#endregion

var serviceProvider = services.BuildServiceProvider();
var executor = serviceProvider.GetRequiredService<CommandExecutor>();
return await executor.Execute();
