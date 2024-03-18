using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Sidekick.Common.Blazor.Layouts;
using Sidekick.Common.Blazor.Views;
using Sidekick.Common.Cache;
using Sidekick.Common.Settings;
using Sidekick.Modules.Settings.Localization;

namespace Sidekick.Modules.Settings
{
    public class SettingsLayout : MenuLayout
    {
        [Inject]
        private SettingsResources Resources { get; set; }

        [Inject]
        private ISettingsService SettingsService { get; set; }

        [Inject]
        private ISettings Settings { get; set; }

        [Inject]
        private NavigationManager NavigationManager { get; set; }

        [Inject]
        private SettingsModel ViewModel { get; set; }

        [Inject]
        private ICacheProvider CacheProvider { get; set; }

        [Inject]
        private IViewLocator ViewLocator { get; set; }

        /// <inheritdoc/>
        protected override async Task OnInitializedAsync()
        {
            MenuLinks = new()
            {
                new()
                {
                    Name= Resources.General,
                    Url="/settings",
                },
                new()
                {
                    Name= Resources.PriceCheck,
                    Url="/settings/price",
                },
                new()
                {
                    Name= Resources.Map,
                    Url="/settings/map",
                },
                new()
                {
                    Name= Resources.Wiki,
                    Url="/settings/wiki",
                },
                new()
                {
                    Name= Resources.Chat_Commands,
                    Url="/settings/chat",
                },
                new()
                {
                    Name= Resources.WealthTracker,
                    Url="/settings/wealth",
                },
            };

            MenuIcon = false;

            FooterActions = new()
            {
                new()
                {
                    Name = Resources.ResetCache,
                    OnClick = ResetCache,
                    Variant = MudBlazor.Variant.Text,
                    Color = MudBlazor.Color.Default,
                },
                new()
                {
                    Name = Resources.Save,
                    OnClick = Save,
                    Variant = MudBlazor.Variant.Filled,
                    Color = MudBlazor.Color.Primary,
                },
            };

            await base.OnInitializedAsync();
        }

        public async Task Save()
        {
            ViewModel.Bearer_Token = Settings.Bearer_Token; // Keep the token from the settings.
            await SettingsService.Save(ViewModel);
            await Wrapper.View.Close();
        }

        public async Task ResetCache()
        {
            CacheProvider.Clear();
            await ViewLocator.Open("/initialize");
            await Wrapper.View?.Close();
        }
    }
}
