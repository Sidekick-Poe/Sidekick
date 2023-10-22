using System.Diagnostics;
using Microsoft.AspNetCore.Components;
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
        private ILogger<Update> Logger { get; set; }

        [Inject]
        private IGitHubClient GitHubClient { get; set; }

        [Inject]
        private IApplicationService ApplicationService { get; set; }

        [Inject]
        private UpdateResources UpdateResources { get; set; }

        [Inject]
        private ICacheProvider CacheProvider { get; set; }

        public override string Title => UpdateResources.Title;
        public override SidekickViewType ViewType => SidekickViewType.Modal;

        private string Step { get; set; }
        private bool Error { get; set; }

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();

            try
            {
                // Checking release
                Step = UpdateResources.Downloading;
                StateHasChanged();

                var release = await GitHubClient.GetLatestRelease();
                if (!release.IsNewerVersion || !release.IsExecutable)
                {
                    Step = UpdateResources.NotAvaialble;
                    StateHasChanged();
                    return;
                }

                // Downloading
                Step = UpdateResources.Downloading;
                StateHasChanged();
                var path = SidekickPaths.GetDataFilePath("Sidekick-Update.exe");
                if (!await GitHubClient.DownloadLatest(path))
                {
                    Step = UpdateResources.Failed;
                    StateHasChanged();
                    return;
                }

                CacheProvider.Clear();

                // Downloaded
                Step = UpdateResources.Downloaded;
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
