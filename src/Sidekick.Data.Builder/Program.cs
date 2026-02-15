using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Sidekick.Data;
using Sidekick.Data.Files;
using Sidekick.Data.Game;
using Sidekick.Data.Ninja;
using Sidekick.Data.Options;
using Sidekick.Data.Trade;

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
services.AddSingleton<NinjaDownloader>();
services.AddSingleton<TradeDownloader>();
services.AddSingleton<RepoeDownloader>();
services.AddSingleton<DataFileWriter>();

services.Configure<DataOptions>(opt =>
{
    for (var i = 0; i < args.Length; i++)
    {
        var a = args[i];
        switch (a)
        {
            case "--folder" when i + 1 < args.Length:
                opt.DataFolder = args[++i];
                break;
            case "--poe1" when i + 1 < args.Length:
                opt.Poe1League = args[++i];
                break;
            case "--poe2" when i + 1 < args.Length:
                opt.Poe2League = args[++i];
                break;
            case "--timeout" when i + 1 < args.Length && int.TryParse(args[++i], out var t):
                opt.TimeoutSeconds = t;
                break;
        }
    }
});

#endregion

var serviceProvider = services.BuildServiceProvider();
var executor = serviceProvider.GetRequiredService<CommandExecutor>();
return await executor.Execute(args);
