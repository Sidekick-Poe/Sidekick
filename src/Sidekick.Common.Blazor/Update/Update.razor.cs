using System.Diagnostics;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using Sidekick.Apis.GitHub;
using Sidekick.Common.Blazor.Views;
using Sidekick.Common.Cache;
using Sidekick.Common.Platform;

namespace Sidekick.Common.Blazor.Update
{
    public partial class Update : SidekickView
    {
        [Inject]
        private ILogger<Update> Logger { get; set; } = null!;

        [Inject]
        private IGitHubClient GitHubClient { get; set; } = null!;

        [Inject]
        private IApplicationService ApplicationService { get; set; } = null!;

        [Inject]
        private IStringLocalizer<Update> Resources { get; set; } = null!;

        [Inject]
        private ICacheProvider CacheProvider { get; set; } = null!;

        public override string Title => Resources["Update"];
        public override SidekickViewType ViewType => SidekickViewType.Modal;

        private string? Step { get; set; }
        private bool Error { get; set; }

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();

            try
            {
                // Checking release
                Step = Resources["Downloading the latest version..."];
                StateHasChanged();

                var release = await GitHubClient.GetLatestRelease();
                if (!release.IsNewerVersion || !release.IsExecutable)
                {
                    Step = Resources["No update has been found to download. You can try to download the latest version manually from https://sidekick-poe.github.io/"];
                    StateHasChanged();
                    return;
                }

                // Downloading
                Step = Resources["Downloading the latest version..."];
                StateHasChanged();
                var path = SidekickPaths.GetDataFilePath("Sidekick-Update.exe");
                if (!await GitHubClient.DownloadLatest(path))
                {
                    Step = Resources["The update has failed to install automatically. You should try to download and install the update manually from https://sidekick-poe.github.io/"];
                    StateHasChanged();
                    return;
                }

                CacheProvider.Clear();

                // Downloaded
                Step = Resources["Download ready! Starting installer..."];
                StateHasChanged();
                await Task.Delay(1500);
                Process.Start(path);
                ApplicationService.Shutdown();
            }
            catch (Exception ex)
            {
                Logger.LogError(ex.Message);
                Error = true;
            }
        }
    }
}
