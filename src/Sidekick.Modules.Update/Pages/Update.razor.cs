using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;
using Sidekick.Apis.GitHub;
using Sidekick.Common.Blazor.Views;
using Sidekick.Common.Cache;
using Sidekick.Common.Platform;
using Sidekick.Modules.Update.Localization;

namespace Sidekick.Modules.Update.Pages
{
    public partial class Update : SidekickView
    {
        [Inject] private ILogger<Update> Logger { get; set; }
        [Inject] private IGitHubClient GitHubClient { get; set; }
        [Inject] private IApplicationService ApplicationService { get; set; }
        [Inject] private UpdateResources UpdateResources { get; set; }
        [Inject] private ICacheProvider CacheProvider { get; set; }

        public override string Title => "Update";
        public override SidekickViewType ViewType => SidekickViewType.Modal;

        private string Step { get; set; }
        private bool Error { get; set; }

        public static bool HasRun { get; set; } = false;

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
            await Handle();
        }

        public async Task Handle()
        {
#if DEBUG
            Step = "Development mode detected, redirecting to setup.";
            await Task.Delay(750);
            NavigationManager.NavigateTo("/setup");
            return;
#endif

            try
            {
                // Checking release
                Step = UpdateResources.Checking;
                StateHasChanged();
                var release = await GitHubClient.GetLatestRelease();

                if (release == null || !GitHubClient.IsUpdateAvailable(release))
                {
                    await Task.Delay(750);
                    NavigationManager.NavigateTo("/setup");
                    return;
                }

                // Downloading
                Step = UpdateResources.Downloading(release.Tag);
                StateHasChanged();
                var path = await GitHubClient.DownloadRelease(release);
                if (path == null)
                {
                    Step = UpdateResources.Failed;
                    StateHasChanged();
                    await Task.Delay(3000);
                    NavigationManager.NavigateTo("/setup");
                    return;
                }

                CacheProvider.Clear();

                // Downloaded
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
