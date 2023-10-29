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
        private NavigationManager NavigationManager { get; set; }

        [Inject]
        private SettingsModel ViewModel { get; set; }

        [Inject]
        private ICacheProvider CacheProvider { get; set; }

        [Inject]
        private IViewLocator ViewLocator { get; set; }

        [CascadingParameter]
        public SidekickView View { get; set; }

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
                    Name= Resources.Cheatsheets,
                    Url="/settings/cheatsheets",
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

            var routeMatch = new System.Text.RegularExpressions.Regex("cheatsheets\\/([^\\\\\\/]*)").Match(NavigationManager.Uri);
            if (routeMatch.Success)
            {
                await SettingsService.Save(nameof(ISettings.Cheatsheets_Selected), routeMatch.Groups[1].Value);
            }

            await base.OnInitializedAsync();
        }

        public async Task Save()
        {
            await SettingsService.Save(ViewModel);
            await Wrapper.View.Close();
        }

        public async Task ResetCache()
        {
            CacheProvider.Clear();
            await ViewLocator.Open("/initialize");
            await View.Close();
        }
    }
}
