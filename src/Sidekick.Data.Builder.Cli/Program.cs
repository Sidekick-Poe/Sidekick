using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Sidekick.Common;
using Sidekick.Data;
using Sidekick.Data.Builder;
using Sidekick.Data.Languages;
using Options = Sidekick.Data.Builder.Cli.Options;

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
        }
    }
});

#endregion

var serviceProvider = services.BuildServiceProvider();

try
{
    var options = serviceProvider.GetRequiredService<IOptions<Options>>();
    var dataBuilder = serviceProvider.GetRequiredService<DataBuilder>();
    if (string.IsNullOrEmpty(options.Value.Language))
    {
        await dataBuilder.DownloadAndBuildAll();
    }
    else
    {
        var gameLanguageProvider = serviceProvider.GetRequiredService<IGameLanguageProvider>();
        await dataBuilder.Download(gameLanguageProvider.InvariantLanguage);
        await dataBuilder.BuildInvariant();
        await dataBuilder.Build(gameLanguageProvider.InvariantLanguage);

        if (!string.IsNullOrEmpty(options.Value.Language) && options.Value.Language != gameLanguageProvider.InvariantLanguage.Code)
        {
            var language = gameLanguageProvider.GetList().FirstOrDefault(x => x.Code == options.Value.Language);
            if (language != null)
            {
                await dataBuilder.Download(language);
                await dataBuilder.Build(language);
            }
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
