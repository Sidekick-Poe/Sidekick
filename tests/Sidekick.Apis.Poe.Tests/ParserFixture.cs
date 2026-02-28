using Bunit;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Sidekick.Apis.Common;
using Sidekick.Apis.Poe.Items;
using Sidekick.Apis.Poe.Tests.Mocks;
using Sidekick.Apis.Poe.Trade;
using Sidekick.Apis.Poe.Trade.ApiStats;
using Sidekick.Apis.Poe.Trade.Clients;
using Sidekick.Apis.Poe.Trade.Parser;
using Sidekick.Apis.Poe.Trade.Parser.Properties;
using Sidekick.Apis.Poe.Trade.Parser.Stats;
using Sidekick.Apis.Poe.Trade.Trade.Filters;
using Sidekick.Apis.Poe.Trade.Trade.Filters.AutoSelect;
using Sidekick.Apis.PoeNinja;
using Sidekick.Apis.PoeWiki;
using Sidekick.Common;
using Sidekick.Common.Browser;
using Sidekick.Common.Database;
using Sidekick.Common.Initialization;
using Sidekick.Common.Settings;
using Sidekick.Data;
using Sidekick.Data.Items.Models;
using Sidekick.Data.Languages;
using Xunit;

namespace Sidekick.Apis.Poe.Tests;

public abstract class ParserFixture : IAsyncLifetime
{
    protected abstract GameType GameType { get; }
    protected abstract string Language { get; }

    private Task? initializationTask;

    public IItemParser Parser { get; private set; } = null!;
    public ICurrentGameLanguage CurrentGameLanguage { get; private set; } = null!;
    public ITradeFilterProvider TradeFilterProvider { get; private set; } = null!;
    public IPropertyParser PropertyParser { get; private set; } = null!;
    public ISettingsService SettingsService { get; private set; } = null!;
    public IApiStatsProvider ApiStatsProvider { get; private set; } = null!;
    public IStatParser StatParser { get; private set; } = null!;
    private TestContext TestContext { get; set; } = null!;

    public async Task InitializeAsync()
    {
        TestContext = new TestContext();
        TestContext.Services.AddLocalization();

        TestContext.Services
            // Building blocks
            .AddSidekickCommon(SidekickApplicationType.Test)
            .AddSidekickCommonBrowser()
            .AddSidekickCommonDatabase(SidekickPaths.DatabasePath)
            .AddSidekickData()

            // Apis
            .AddSidekickCommonApi()
            .AddSidekickPoeTradeApi()
            .AddSidekickPoeNinjaApi()
            .AddSidekickPoeWikiApi();

        // Override the Trade API client in tests to always use local fallback data files
        TestContext.Services.AddTransient<ITradeApiClient, TestTradeApiClient>();
        TestContext.Services.AddSingleton<ISettingsService, TestSettingsService>();

        SettingsService = TestContext.Services.GetRequiredService<ISettingsService>();
        await SettingsService.Set(SettingKeys.LanguageParser, Language);
        await SettingsService.Set(SettingKeys.LanguageUi, Language);
        await SettingsService.Set(AutoSelectPreferences.DefaultFillMinSettingKey, true);
        await SettingsService.Set(AutoSelectPreferences.DefaultNormalizeBySettingKey, 0.1);
        if (GameType == GameType.PathOfExile1) await SettingsService.Set(SettingKeys.LeagueId, "poe1.Standard");
        else if (GameType == GameType.PathOfExile2) await SettingsService.Set(SettingKeys.LeagueId, "poe2.Standard");

        if (initializationTask == null)
        {
            var serviceProvider = TestContext.Services.GetRequiredService<IServiceProvider>();
            initializationTask = Initialize(serviceProvider);
        }

        await initializationTask;

        Parser = TestContext.Services.GetRequiredService<IItemParser>();
        CurrentGameLanguage = TestContext.Services.GetRequiredService<ICurrentGameLanguage>();
        PropertyParser = TestContext.Services.GetRequiredService<IPropertyParser>();
        TradeFilterProvider = TestContext.Services.GetRequiredService<ITradeFilterProvider>();
        ApiStatsProvider = TestContext.Services.GetRequiredService<IApiStatsProvider>();
        StatParser = TestContext.Services.GetRequiredService<IStatParser>();
    }

    public Task DisposeAsync()
    {
        TestContext.Dispose();
        return Task.CompletedTask;
    }

    private static async Task Initialize(IServiceProvider serviceProvider)
    {
        var logger = serviceProvider.GetRequiredService<ILogger<ParserFixture>>();
        var configuration = serviceProvider.GetRequiredService<IOptions<SidekickConfiguration>>();
        List<IInitializableService> services = [];
        foreach (var serviceType in configuration.Value.InitializableServices)
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

    public void AssertHasStat(Item actual, StatCategory expectedCategory, string expectedText, params double[] expectedValues)
    {
#if DEBUG
        var texts = (from stat in actual.Stats
            from pattern in stat.MatchedPatterns
            from tradeId in pattern.TradeIds
            let text = ApiStatsProvider.Definitions[new StatKey(tradeId, pattern.Option?.Id)].Text
            select $"{stat.Category} - {text}").ToList();
#endif

        var actualStat = (from stat in actual.Stats
            from pattern in stat.MatchedPatterns
            from tradeId in pattern.TradeIds
            let text = ApiStatsProvider.Definitions[new StatKey(tradeId, pattern.Option?.Id)].Text
            where stat.Category == expectedCategory && text == expectedText
            select stat).FirstOrDefault();

        Assert.NotNull(actualStat);

        if (expectedValues.Length != 0)
        {
            for (var i = 0; i < expectedValues.Length; i++)
            {
                AssertExtensions.AssertCloseEnough(expectedValues[i], actualStat?.Values[i]);
            }

            AssertExtensions.AssertCloseEnough(expectedValues.Average(), actualStat?.AverageValue);
        }
    }

}
