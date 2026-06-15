using Bunit;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Sidekick.Apis.Common;
using Sidekick.Apis.Poe.Tests.Mocks;
using Sidekick.Apis.Poe.Trade;
using Sidekick.Apis.Poe.Trade.Clients;
using Sidekick.Apis.Poe.Trade.Filters;
using Sidekick.Apis.Poe.Trade.Filters.AutoSelect;
using Sidekick.Apis.Poe.Trade.Parser;
using Sidekick.Apis.Poe.Trade.Parser.Properties;
using Sidekick.Apis.Poe.Trade.Parser.Stats;
using Sidekick.Apis.PoeNinja;
using Sidekick.Apis.PoeWiki;
using Sidekick.Common;
using Sidekick.Common.Database;
using Sidekick.Common.Initialization;
using Sidekick.Common.Settings;
using Sidekick.Data;
using Sidekick.Data.Builder;
using Sidekick.Data.Items;
using Sidekick.Data.Languages;
using Sidekick.Data.Stats;
using Xunit;
using TradeFilter=Sidekick.Apis.Poe.Trade.Filters.Types.TradeFilter;

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
    public IStatParser StatParser { get; private set; } = null!;
    protected TestContext TestContext { get; set; } = null!;

    public virtual async Task InitializeAsync()
    {
        TestContext = new TestContext();
        TestContext.Services.AddLocalization();

        TestContext.Services
            // Building blocks
            .AddSidekickCommon(SidekickApplicationType.Test)
            .AddSidekickCommonDatabase(SidekickPaths.DatabasePath)
            .AddSidekickData()
            .AddSidekickDataBuilder()

            // Apis
            .AddSidekickCommonApi()
            .AddSidekickPoeTradeApi()
            .AddSidekickPoeNinjaApi()
            .AddSidekickPoeWikiApi();

        // Override the Trade API client in tests to always use local fallback data files
        TestContext.Services.AddTransient<ITradeApiClient, TestTradeApiClient>();
        TestContext.Services.AddSingleton<ISettingsService, TestSettingsService>();
        TestContext.Services.AddSingleton<RawDataProvider>();

        RegisterServices(TestContext.Services);

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
        StatParser = TestContext.Services.GetRequiredService<IStatParser>();
    }

    public Task DisposeAsync()
    {
        TestContext.Dispose();
        return Task.CompletedTask;
    }

    protected virtual void RegisterServices(IServiceCollection services) {}

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

    public async Task<List<TradeFilter>> GetPropertyFilters(Item item)
    {
        var filters = await PropertyParser.GetFilters(item);
        return filters.Flatten();
    }

    public Stat? AssertHasStat(Item actual, StatCategory expectedCategory, string expectedText, params double[] expectedValues)
    {
        return AssertHasStat(actual, expectedCategory, expectedText, null, false, expectedValues);
    }

    public Stat? AssertHasStat(Item actual, StatCategory expectedCategory, string expectedText, string? expectedOptionText, params double[] expectedValues)
    {
        return AssertHasStat(actual, expectedCategory, expectedText, expectedOptionText, false, expectedValues);
    }

    public Stat? AssertHasFuzzyStat(Item actual, StatCategory expectedCategory, string expectedText, params double[] expectedValues)
    {
        return AssertHasStat(actual, expectedCategory, expectedText, null, true, expectedValues);
    }

    public Stat? AssertHasFuzzyStat(Item actual, StatCategory expectedCategory, string expectedText, string? expectedOptionText, params double[] expectedValues)
    {
        return AssertHasStat(actual, expectedCategory, expectedText, expectedOptionText, true, expectedValues);
    }

    private Stat? AssertHasStat(Item actual, StatCategory expectedCategory, string expectedText, string? expectedOptionText, bool fuzzy, params double[] expectedValues)
    {
        var actualStat = FindStat(actual, expectedCategory, expectedText, expectedOptionText, fuzzy);
        if (actualStat == null)
        {
            Assert.Fail("The actual stat does not exist. Expected: " + expectedText + " - " + expectedOptionText);
            return null;
        }

        if (expectedValues.Length == 0) return actualStat;

        for (var i = 0; i < expectedValues.Length; i++)
        {
            if (actualStat?.Values.Count <= i) Assert.Fail("The actual stat does not have enough values");
            AssertExtensions.AssertCloseEnough(expectedValues[i], actualStat?.Values[i]);
        }

        AssertExtensions.AssertCloseEnough(expectedValues.Average(), actualStat?.AverageValue);

        return actualStat;
    }

    private Stat? FindStat(Item actual, StatCategory expectedCategory, string expectedText, string? expectedOptionText, bool? fuzzy)
    {
        foreach (var stat in actual.Stats)
        {
            switch (fuzzy)
            {
                case false when stat.MatchedFuzzily:
                case true when !stat.MatchedFuzzily:
                    continue;
                case null:
                    break;
            }

            if (stat.Category != expectedCategory) continue;

            var tradeStatDefinitions = stat.Definitions
                .Where(x => x.TradeIds != null)
                .SelectMany(x => x.TradeIds!)
                .Distinct()
                .Select(x => StatParser.TradeDefinitions.GetValueOrDefault(x))
                .Where(x => x != null)
                .SelectMany(x => x!)
                .ToList();
            foreach (var tradeStatDefinition in tradeStatDefinitions)
            {
                if (tradeStatDefinition.Text != expectedText) continue;
                if (expectedOptionText == null) return stat;

                if (tradeStatDefinition.OptionText == null) continue;
                if (tradeStatDefinition.OptionText == expectedOptionText) return stat;
            }

            foreach (var gameStat in stat.Definitions
                         .Where(x => x.TradeIds == null))
            {
                if (gameStat.Text != expectedText) continue;
                if (expectedOptionText == null) return stat;
            }
        }

        return null;
    }

    public void AssertDoesNotHaveStat(Item actual, StatCategory expectedCategory, string expectedText, string? expectedOptionText = null)
    {
        var actualStat = FindStat(actual, expectedCategory, expectedText, expectedOptionText, null);
        Assert.Null(actualStat);
    }

    public void AssertHasPseudoStat(Item actual, string expectedText, double? expectedValue = null)
    {
        var actualStat = actual.PseudoStats.FirstOrDefault(x => expectedText == x.Text);
        Assert.NotNull(actualStat);
        Assert.Equal(expectedText, actualStat.Text);

        if (expectedValue != null) AssertExtensions.AssertCloseEnough(expectedValue.Value, actualStat.Value);
    }
}
