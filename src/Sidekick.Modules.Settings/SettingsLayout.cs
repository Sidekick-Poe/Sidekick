using Microsoft.AspNetCore.Components;
using Sidekick.Common.Blazor.Layouts;
using Sidekick.Common.Cache;
using Sidekick.Modules.Settings.Localization;

namespace Sidekick.Modules.Settings
{
    public class SettingsLayout : MenuLayout
    {
        [Inject]
        private SettingsResources Resources { get; set; } = null!;

        [Inject]
        private ICacheProvider CacheProvider { get; set; } = null!;

        [Inject]
        private NavigationManager NavigationManager { get; set; } = null!;

        /// <inheritdoc/>
        protected override async Task OnInitializedAsync()
        {
            MenuLinks =
            [
                new()
                {
                    Name = Resources.General,
                    Url = "/settings",
                },

                new()
                {
                    Name = Resources.PriceCheck,
                    Url = "/settings/price",
                },

                new()
                {
                    Name = Resources.Map,
                    Url = "/settings/map",
                },

                new()
                {
                    Name = Resources.Wiki,
                    Url = "/settings/wiki",
                },

                new()
                {
                    Name = Resources.Chat_Commands,
                    Url = "/settings/chat",
                },

                new()
                {
                    Name = Resources.WealthTracker,
                    Url = "/settings/wealth",
                },];

            MenuIcon = false;

            FooterActions =
            [
                new()
                {
                    Name = Resources.ResetCache,
                    OnClick = ResetCache,
                    Variant = MudBlazor.Variant.Text,
                    Color = MudBlazor.Color.Default,
                },];

            await base.OnInitializedAsync();
        }

        public Task ResetCache()
        {
            CacheProvider.Clear();
            NavigationManager.NavigateTo("/initialize");
            return Task.CompletedTask;
        }
    }
}
