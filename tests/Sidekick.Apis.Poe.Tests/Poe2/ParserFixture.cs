using Bunit;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Sidekick.Apis.Poe.Modifiers;
using Sidekick.Apis.PoeNinja;
using Sidekick.Apis.PoeWiki;
using Sidekick.Common;
using Sidekick.Common.Cache;
using Sidekick.Common.Database;
using Sidekick.Common.Initialization;
using Sidekick.Common.Settings;
using Sidekick.Mock;
using Xunit;

namespace Sidekick.Apis.Poe.Tests.Poe2
{
    public class ParserFixture : IAsyncLifetime
    {
        private static Task? initializationTask;

        public IInvariantModifierProvider InvariantModifierProvider { get; private set; } = null!;

        public IItemParser Parser { get; private set; } = null!;

        public Task DisposeAsync()
        {
            return Task.CompletedTask;
        }

        public async Task InitializeAsync()
        {
            using var ctx = new TestContext();
            ctx.Services.AddLocalization();

            ctx.Services
                // Building blocks
                .AddSidekickCommon()
                .AddSidekickCommonDatabase(SidekickPaths.DatabasePath)

                // Apis
                .AddSidekickPoeApi()
                .AddSidekickPoeNinjaApi()
                .AddSidekickPoeWikiApi()

                // Mocks
                .AddSidekickMocks();

            var settingsService = ctx.Services.GetRequiredService<ISettingsService>();
            await settingsService.Set(SettingKeys.LanguageParser, "en");
            await settingsService.Set(SettingKeys.LanguageUi, "en");
            await settingsService.Set(SettingKeys.LeagueId, "poe2.Standard");

            if (initializationTask == null)
            {
                var serviceProvider = ctx.Services.GetRequiredService<IServiceProvider>();
                initializationTask = Initialize(serviceProvider);
            }

            await initializationTask;

            Parser = ctx.Services.GetRequiredService<IItemParser>();
            InvariantModifierProvider = ctx.Services.GetRequiredService<IInvariantModifierProvider>();
        }

        private async Task Initialize(IServiceProvider serviceProvider)
        {
            var  cache = serviceProvider.GetRequiredService<ICacheProvider>();
            await cache.Clear();

            var  configuration = serviceProvider.GetRequiredService<IOptions<SidekickConfiguration>>();
            var  logger = serviceProvider.GetRequiredService<ILogger<ParserFixture>>();
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
}
