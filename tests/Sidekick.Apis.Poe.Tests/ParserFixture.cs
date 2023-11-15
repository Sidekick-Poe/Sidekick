using System.Threading.Tasks;
using Bunit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Sidekick.Apis.Poe.Modifiers;
using Sidekick.Apis.PoeNinja;
using Sidekick.Apis.PoeWiki;
using Sidekick.Common;
using Sidekick.Common.Blazor;
using Sidekick.Common.Settings;
using Sidekick.Mock;
using Sidekick.Modules.Settings;
using Xunit;

namespace Sidekick.Apis.Poe.Tests
{
    public class ParserFixture : IAsyncLifetime
    {
        public IInvariantModifierProvider InvariantModifierProvider { get; private set; }

        public IItemParser Parser { get; private set; }

        public Task DisposeAsync()
        {
            return Task.CompletedTask;
        }

        public async Task InitializeAsync()
        {
            using var ctx = new TestContext();
            var configuration = new ConfigurationBuilder()
                .AddJsonFile(SidekickPaths.GetDataFilePath(SettingsService.FileName), true, true)
                .Build();

            ctx.Services.AddLocalization();

            ctx.Services
                // Building blocks
                .AddSidekickCommon(configuration)
                .AddSidekickCommonBlazor()

                // Apis
                .AddSidekickPoeApi()
                .AddSidekickPoeNinjaApi()
                .AddSidekickPoeWikiApi()

                // Modules
                .AddSidekickSettings()

                // Mocks
                .AddSidekickMocks();

            var settingsService = ctx.Services.GetRequiredService<ISettingsService>();
            await settingsService.Save(nameof(ISettings.Language_Parser), "en");
            await settingsService.Save(nameof(ISettings.Language_UI), "en");
            await settingsService.Save(nameof(ISettings.LeagueId), "Standard");

            var initComponent = ctx.RenderComponent<Common.Blazor.Initialization.Initialization>();
            await initComponent.Instance.InitializationTask;

            Parser = ctx.Services.GetRequiredService<IItemParser>();
            InvariantModifierProvider = ctx.Services.GetRequiredService<IInvariantModifierProvider>();
        }
    }
}
