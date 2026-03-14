using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Sidekick.Common;
using Sidekick.Data;
using Sidekick.Data.Builder;
using Sidekick.Data.Languages;
using Options=Sidekick.Data.Builder.Cli.Options;

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
services.AddSidekickData();
services.AddSidekickDataBuilder();

services.Configure<Options>(opt =>
{
    for (var i = 0; i < args.Length; i++)
    {
        var a = args[i];
        switch (a)
        {
            case "--language" when i + 1 < args.Length:
                opt.Language = args[++i];
                break;
            case "--poe1" when i + 1 < args.Length:
                opt.Poe1 = args[++i] != "false";
                break;
            case "--poe2" when i + 1 < args.Length:
                opt.Poe2 = args[++i] != "false";
                break;
            case "--stats":
                opt.Stats = true;
                break;
            case "--trade":
                opt.Trade = true;
                break;
            case "--repoe":
                opt.Repoe = true;
                break;
            case "--pseudo":
                opt.Pseudo = true;
                break;
            case "--ninja":
                opt.Ninja = true;
                break;
        }
    }
});

#endregion

var serviceProvider = services.BuildServiceProvider();

try
{
    var options = serviceProvider.GetRequiredService<IOptions<Options>>();
    var dataBuilder = serviceProvider.GetRequiredService<DataBuilder>();
    var dataProvider = serviceProvider.GetRequiredService<DataProvider>();

    if (string.IsNullOrEmpty(options.Value.Language))
    {
        if (options.Value.HasSelectiveOptions)
        {
            await dataBuilder.DownloadAndBuildAll(stats: options.Value.Stats,
                                                  trade: options.Value.Trade,
                                                  repoe: options.Value.Repoe,
                                                  pseudo: options.Value.Pseudo,
                                                  ninja: options.Value.Ninja);
        }
        else
        {
            dataProvider.DeleteAll();
            await dataBuilder.DownloadAndBuildAll();
        }
    }
    else
    {
        var gameLanguageProvider = serviceProvider.GetRequiredService<IGameLanguageProvider>();
        var language = gameLanguageProvider.GetList().FirstOrDefault(x => x.Code == options.Value.Language);
        if (language == null) { throw new Exception($"Language {options.Value.Language} not found."); }

        if (options.Value.HasSelectiveOptions)
        {
            await dataBuilder.DownloadAndBuild(language, stats: options.Value.Stats,
                                               trade: options.Value.Trade,
                                               repoe: options.Value.Repoe,
                                               pseudo: options.Value.Pseudo,
                                               ninja: options.Value.Ninja);
        }
        else
        {
            await dataBuilder.DownloadAndBuild(language);
        }
    }

    return 0;
}
catch (Exception ex)
{
    var logger = serviceProvider.GetRequiredService<ILogger<Program>>();
    logger.LogCritical(ex, "Failed to execute command.");
    return 2;
}
