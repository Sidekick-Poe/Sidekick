using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Sidekick.Common.Blazor.Layouts;
using Sidekick.Common.Extensions;
using Sidekick.Common.Settings;

namespace Sidekick.Modules.Cheatsheets
{
    public class CheatsheetLayout : MenuLayout
    {
        [Inject]
        private ISettings Settings { get; set; }

        [Inject]
        private ISettingsService SettingsService { get; set; }

        [Inject]
        private NavigationManager NavigationManager { get; set; }

        /// <inheritdoc/>
        protected override async Task OnInitializedAsync()
        {
            MenuLinks = Settings.Cheatsheets_Pages.Select(x => new MenuLink()
            {
                Name = x.Name,
                Url = $"/cheatsheets/{x.Name.EncodeBase64Url()}",
            })
            .ToList();

            var routeMatch = new System.Text.RegularExpressions.Regex("cheatsheets\\/([^\\\\\\/]*)").Match(NavigationManager.Uri);
            if (routeMatch.Success)
            {
                await SettingsService.Save(nameof(ISettings.Cheatsheets_Selected), routeMatch.Groups[1].Value);
            }

            await base.OnInitializedAsync();
        }
    }
}
