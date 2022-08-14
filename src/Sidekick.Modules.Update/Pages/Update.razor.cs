using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Sidekick.Apis.GitHub;
using Sidekick.Common;
using Sidekick.Common.Blazor.Views;
using Sidekick.Common.Cache;
using Sidekick.Common.Platform.Tray;
using Sidekick.Modules.Update.Localization;

namespace Sidekick.Modules.Update.Pages
{
    public partial class Update : ComponentBase
    {
        [Inject] private ILogger<Update> Logger { get; set; }
        [Inject] private IGitHubClient GitHubClient { get; set; }
        [Inject] private IAppService AppService { get; set; }
        [Inject] private UpdateResources UpdateResources { get; set; }
        [Inject] private IWebHostEnvironment Environment { get; set; }
        [Inject] private NavigationManager NavigationManager { get; set; }
        [Inject] private IViewInstance ViewInstance { get; set; }
        [Inject] private ICacheProvider CacheProvider { get; set; }
        [Inject] private ITrayProvider TrayProvider { get; set; }

        private string Title { get; set; }

        public static bool HasRun { get; set; } = false;

        protected override async Task OnInitializedAsync()
        {
            await ViewInstance.Initialize("Update", width: 400, height: 230, isModal: true);
            await base.OnInitializedAsync();
            await Handle();
        }

        public async Task Handle()
        {
            try
            {
                // Checking release
                Title = UpdateResources.Checking;
                StateHasChanged();
                var release = await GitHubClient.GetLatestRelease();

                if (release == null || !GitHubClient.IsUpdateAvailable(release) || Environment.IsDevelopment())
                {
                    await Task.Delay(750);
                    NavigationManager.NavigateTo("/setup");
                    return;
                }

                // Downloading
                Title = UpdateResources.Downloading(release.Tag);
                StateHasChanged();
                var path = await GitHubClient.DownloadRelease(release);
                if (path == null)
                {
                    Title = UpdateResources.Failed;
                    StateHasChanged();
                    await Task.Delay(3000);
                    NavigationManager.NavigateTo("/setup");
                    return;
                }

                CacheProvider.Clear();

                // Downloaded
                await Task.Delay(1500);
                Process.Start(path);
                Exit();
            }
            catch (Exception ex)
            {
                Logger.LogError(ex.Message);
                TrayProvider.SendNotification(UpdateResources.Failed);
                NavigationManager.NavigateTo("/setup");
            }
        }

        public void Exit()
        {
            AppService.Shutdown();
        }
    }
}
