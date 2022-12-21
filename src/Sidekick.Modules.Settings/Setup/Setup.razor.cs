using System;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using Sidekick.Apis.Poe;
using Sidekick.Common;
using Sidekick.Common.Blazor.Views;
using Sidekick.Common.Cache;
using Sidekick.Common.Game.Languages;
using Sidekick.Common.Platform.Tray;
using Sidekick.Common.Settings;
using Sidekick.Modules.Settings.Components;
using Sidekick.Modules.Settings.Localization;

namespace Sidekick.Modules.Settings.Setup
{
    public partial class Setup : ComponentBase
    {
        [Inject] private SettingsResources SettingsResources { get; set; }
        [Inject] private SetupResources Resources { get; set; }
        [Inject] private SettingsModel ViewModel { get; set; }
        [Inject] private ISettingsService SettingsService { get; set; }
        [Inject] private IGameLanguageProvider GameLanguageProvider { get; set; }
        [Inject] private IAppService AppService { get; set; }
        [Inject] private NavigationManager NavigationManager { get; set; }
        [Inject] private ILeagueProvider LeagueProvider { get; set; }
        [Inject] private ISettings Settings { get; set; }
        [Inject] private ICacheProvider CacheProvider { get; set; }
        [Inject] private IViewInstance ViewInstance { get; set; }
        [Inject] private ITrayProvider TrayProvider { get; set; }

        public static bool HasRun { get; set; } = false;
        public bool RequiresSetup { get; set; } = false;
        private bool Success { get; set; }
        private MudForm Form { get; set; }

        private LeagueSelect RefLeagueSelect;

        protected override async Task OnInitializedAsync()
        {
            await ViewInstance.Initialize("Setup", width: 400, height: 230, isModal: true);

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
                await ViewInstance.Initialize("Setup", width: 600, height: 715, isModal: true);

                RequiresSetup = true;
                TrayProvider.SendNotification(Resources.NewLeagues);
            }
            else
            {
                NavigationManager.NavigateTo("/initialize");
            }

            await base.OnInitializedAsync();
        }

        public void Exit()
        {
            AppService.Shutdown();
        }

        public async Task Save()
        {
            await Form.Validate();
            if (!Success)
            {
                return;
            }

            await SettingsService.Save(ViewModel);
            NavigationManager.NavigateTo("/initialize");
        }

        public async Task OnGameLanguageChange(string value)
        {
            ViewModel.Language_Parser = value;
            GameLanguageProvider.SetLanguage(value);
            await RefLeagueSelect.RefreshOptions();
        }
    }
}
