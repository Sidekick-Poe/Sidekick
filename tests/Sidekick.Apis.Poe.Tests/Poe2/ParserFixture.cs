using Bunit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Sidekick.Apis.Poe.Filters;
using Sidekick.Apis.Poe.Modifiers;
using Sidekick.Apis.Poe.Parser.Properties;
using Sidekick.Apis.PoeNinja;
using Sidekick.Apis.PoeWiki;
using Sidekick.Common;
using Sidekick.Common.Cache;
using Sidekick.Common.Database;
using Sidekick.Common.Game.Languages;
using Sidekick.Common.Initialization;
using Sidekick.Common.Settings;
using Xunit;

namespace Sidekick.Apis.Poe.Tests.Poe2;

public class ParserFixture : IAsyncLifetime
{
    private static Task? initializationTask;

    public IInvariantModifierProvider InvariantModifierProvider { get; private set; } = null!;
    public IItemParser Parser { get; private set; } = null!;
    public IGameLanguageProvider GameLanguageProvider { get; private set; } = null!;
    public IFilterProvider FilterProvider { get; private set; } = null!;
    public IPropertyParser PropertyParser { get; private set; } = null!;
    public ITradeFilterService TradeFilterService { get; private set; } = null!;
    public ISettingsService SettingsService { get; private set; } = null!;
    private TestContext TestContext { get; set; } = null!;

    public async Task InitializeAsync()
    {
        TestContext = new TestContext();
        TestContext.Services.AddLocalization();

        TestContext.Services
            // Building blocks
            .AddSidekickCommon()
            .AddSidekickCommonDatabase(SidekickPaths.DatabasePath)

            // Apis
            .AddSidekickPoeApi()
            .AddSidekickPoeNinjaApi()
            .AddSidekickPoeWikiApi();

        SettingsService = TestContext.Services.GetRequiredService<ISettingsService>();
        await SettingsService.Set(SettingKeys.LanguageParser, "en");
        await SettingsService.Set(SettingKeys.LanguageUi, "en");
        await SettingsService.Set(SettingKeys.LeagueId, "poe2.Standard");

        if (initializationTask == null)
        {
            var serviceProvider = TestContext.Services.GetRequiredService<IServiceProvider>();
            initializationTask = Initialize(serviceProvider);
        }

        await initializationTask;

        Parser = TestContext.Services.GetRequiredService<IItemParser>();
        InvariantModifierProvider = TestContext.Services.GetRequiredService<IInvariantModifierProvider>();
        GameLanguageProvider = TestContext.Services.GetRequiredService<IGameLanguageProvider>();
        PropertyParser = TestContext.Services.GetRequiredService<IPropertyParser>();
        FilterProvider = TestContext.Services.GetRequiredService<IFilterProvider>();
        TradeFilterService = TestContext.Services.GetRequiredService<ITradeFilterService>();
    }

    public Task DisposeAsync()
    {
        TestContext.Dispose();
        return Task.CompletedTask;
    }

    private static async Task Initialize(IServiceProvider serviceProvider)
    {
        var cache = serviceProvider.GetRequiredService<ICacheProvider>();
        await cache.Clear();

        var configuration = serviceProvider.GetRequiredService<IOptions<SidekickConfiguration>>();
        var logger = serviceProvider.GetRequiredService<ILogger<ParserFixture>>();
        foreach (var serviceType in configuration.Value.InitializableServices)
        {
            var service = serviceProvider.GetRequiredService(serviceType);
            if (service is not IInitializableService initializableService)
            {
                continue;
            }

            logger.LogInformation($"[Initialization] Initializing {initializableService.GetType().FullName}");
            await initializableService.Initialize();
        }
    }
}
