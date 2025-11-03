using Bunit;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Sidekick.Apis.Common;
using Sidekick.Apis.Poe.Languages;
using Sidekick.Apis.Poe.Trade;
using Sidekick.Apis.Poe.Trade.Filters;
using Sidekick.Apis.Poe.Trade.Modifiers;
using Sidekick.Apis.Poe.Trade.Parser.Modifiers;
using Sidekick.Apis.Poe.Trade.Parser.Properties;
using Sidekick.Apis.PoeNinja;
using Sidekick.Apis.PoeWiki;
using Sidekick.Common;
using Sidekick.Common.Browser;
using Sidekick.Common.Cache;
using Sidekick.Common.Database;
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
    public ISettingsService SettingsService { get; private set; } = null!;
    public IModifierProvider ModifierProvider { get; private set; } = null!;
    public IModifierParser ModifierParser { get; private set; } = null!;
    private TestContext TestContext { get; set; } = null!;

    public async Task InitializeAsync()
    {
        TestContext = new TestContext();
        TestContext.Services.AddLocalization();

        TestContext.Services
            // Building blocks
            .AddSidekickCommon()
            .AddSidekickCommonBrowser()
            .AddSidekickCommonDatabase(SidekickPaths.DatabasePath)

            // Apis
            .AddSidekickCommonApi()
            .AddSidekickPoeApi()
            .AddSidekickPoeTradeApi()
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
        ModifierProvider = TestContext.Services.GetRequiredService<IModifierProvider>();
        ModifierParser = TestContext.Services.GetRequiredService<IModifierParser>();
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

        var logger = serviceProvider.GetRequiredService<ILogger<ParserFixture>>();
        List<IInitializableService> services = [];
        foreach (var serviceType in SidekickConfiguration.InitializableServices)
        {
            var service = serviceProvider.GetRequiredService(serviceType);
            if (service is not IInitializableService initializableService) continue;

            services.Add(initializableService);
        }

        foreach (var service in services.OrderBy(x => x.Priority))
        {
            logger.LogInformation($"[Initialization] Initializing {service.GetType().FullName}");
            await service.Initialize();
        }
    }
}
