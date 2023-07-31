using System;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using Sidekick.Apis.Poe;
using Sidekick.Common.Blazor.Views;
using Sidekick.Common.Cache;
using Sidekick.Common.Game.Languages;
using Sidekick.Common.Platform;
using Sidekick.Common.Settings;
using Sidekick.Modules.Settings.Components;
using Sidekick.Modules.Settings.Localization;

namespace Sidekick.Modules.Settings.Setup
{
    public partial class Setup : SidekickView
    {
        [Inject] private SettingsResources SettingsResources { get; set; }
        [Inject] private SetupResources Resources { get; set; }
        [Inject] private SettingsModel ViewModel { get; set; }
        [Inject] private ISettingsService SettingsService { get; set; }
        [Inject] private IGameLanguageProvider GameLanguageProvider { get; set; }
        [Inject] private IApplicationService ApplicationService { get; set; }
        [Inject] private ILeagueProvider LeagueProvider { get; set; }
        [Inject] private ISettings Settings { get; set; }
        [Inject] private ICacheProvider CacheProvider { get; set; }

        public static bool HasRun { get; set; } = false;
        public bool RequiresSetup { get; set; } = false;
        public bool NewLeagues { get; set; } = false;
        private bool Success { get; set; }
        private MudForm Form { get; set; }

        private LeagueSelect RefLeagueSelect;

        public override string Title => "Setup";
        public override SidekickViewType ViewType => SidekickViewType.Modal;
        public override int ViewHeight => NewLeagues ? 600 : base.ViewHeight;
        public override int ViewWidth => NewLeagues ? 600 : base.ViewWidth;

        protected override async Task OnInitializedAsync()
        {
            var leagues = await LeagueProvider.GetList(false);
            var leaguesHash = Convert.ToBase64String(Encoding.UTF8.GetBytes(JsonSerializer.Serialize(leagues)));

            if (leaguesHash != Settings.LeaguesHash)
            {
                CacheProvider.Clear();
                await SettingsService.Save(nameof(ISettings.LeaguesHash), leaguesHash);
            }

            // Check to see if we should run Setup first before running the rest of the initialization process
            if (string.IsNullOrEmpty(Settings.LeagueId) || !leagues.Any(x => x.Id == Settings.LeagueId))
            {
                RequiresSetup = true;
                NewLeagues = true;
                await ViewLocator.Initialize(this);

                foreach (var command in Settings.Chat_Commands)
                {
                    // Handle a legacy feature that allowed users to use the character name in commands.
                    // This has been removed after the update of Crucible league as the league added the command /leave which leaves the current party.
                    // The requirement of knowing the character name was no longer needed.
                    if (command.Command.Contains("{Me.CharacterName}"))
                    {
                        command.Command = "/leave";
                        await SettingsService.Save(Settings);
                    }
                }
            }
            else
            {
                NavigationManager.NavigateTo("/initialize");
            }

            await base.OnInitializedAsync();
        }

        public void Exit()
        {
            ApplicationService.Shutdown();
        }

        public async Task Save()
        {
            await Form.Validate();
            if (!Success)
            {
                return;
            }

            await SettingsService.Save(ViewModel);
            await Close();
        }

        public async Task OnGameLanguageChange(string value)
        {
            ViewModel.Language_Parser = value;
            GameLanguageProvider.SetLanguage(value);
            await RefLeagueSelect.RefreshOptions();
        }
    }
}
