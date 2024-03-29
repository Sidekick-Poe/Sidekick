using System.Diagnostics;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Sidekick.Common.Blazor.Views;
using Sidekick.Common.Browser;
using Sidekick.Common.Initialization;
using Sidekick.Common.Platform;
using Sidekick.Common.Settings;

namespace Sidekick.Common.Blazor.Initialization
{
    public partial class Initialization : SidekickView
    {
        [Inject]
        private InitializationResources Resources { get; set; } = null!;

        [Inject]
        private ISettings Settings { get; set; } = null!;

        [Inject]
        private ILogger<Initialization> Logger { get; set; } = null!;

        [Inject]
        private IApplicationService ApplicationService { get; set; } = null!;

        [Inject]
        private ITrayProvider TrayProvider { get; set; } = null!;

        [Inject]
        private IOptions<SidekickConfiguration> Configuration { get; set; } = null!;

        [Inject]
        private IServiceProvider ServiceProvider { get; set; } = null!;

        [Inject]
        private IBrowserProvider BrowserProvider { get; set; } = null!;

        private int Count { get; set; }
        private int Completed { get; set; }
        private string? Step { get; set; }
        private int Percentage { get; set; }
        private bool Error { get; set; }

        public Task? InitializationTask { get; set; }
        public override string Title => "Initialize";
        public override SidekickViewType ViewType => SidekickViewType.Modal;

        protected override async Task OnInitializedAsync()
        {
            InitializationTask = Handle();
            await base.OnInitializedAsync();
            await InitializationTask;
        }

        public async Task Handle()
        {
            try
            {
                Completed = 0;
                Count = Configuration.Value.InitializableServices.Count + 1;

                // Report initial progress
                await ReportProgress();

                foreach (var serviceType in Configuration.Value.InitializableServices)
                {
                    var service = ServiceProvider.GetRequiredService(serviceType);
                    if (service is not IInitializableService initializableService)
                    {
                        continue;
                    }

                    Logger.LogInformation($"[Initiazation] Initializing {initializableService.GetType().FullName}");
                    await Run(initializableService.Initialize);
                }

                await Run(InitializeTray);

                // If we have a successful initialization, we delay for half a second to show the
                // "Ready" label on the UI before closing the view
                Completed = Count;
                await ReportProgress();
                await Task.Delay(4000);
                await Close();
            }
            catch (Exception ex)
            {
                Logger.LogError(ex.Message, "[Initiazation] An initialization step failed.");
                Error = true;
            }
        }

        private async Task Run(Func<Task> func)
        {
            // Send the command
            await func.Invoke();

            // Make sure that after all handlers run, the Completed count is updated
            Completed += 1;

            // Report progress
            await ReportProgress();
        }

        private async Task Run(Action action)
        {
            // Send the command
            action.Invoke();

            // Make sure that after all handlers run, the Completed count is updated
            Completed += 1;

            // Report progress
            await ReportProgress();
        }

        private Task ReportProgress()
        {
            return InvokeAsync(() =>
            {
                Percentage = Count == 0 ? 0 : Completed * 100 / Count;
                if (Percentage >= 100)
                {
                    Step = Resources.Ready;
                    Percentage = 100;
                }
                else
                {
                    Step = Resources.Title(Completed, Count);
                }

                StateHasChanged();
                return Task.Delay(100);
            });
        }

        private void InitializeTray()
        {
            var menuItems = new List<TrayMenuItem>();

            menuItems.AddRange(new List<TrayMenuItem>()
            {
                new (label: "Sidekick - " + FileVersionInfo.GetVersionInfo(GetType().Assembly.Location).ProductVersion),
                new (label: "Open the Website", onClick: () =>
                {
                    BrowserProvider.OpenSidekickWebsite();
                    return Task.CompletedTask;
                }),
                new (label: "Wealth", onClick: () => ViewLocator.Open("/wealth")),
                new (label: "Settings", onClick: () => ViewLocator.Open("/settings")),
                new (label: "Exit", onClick: () => { ApplicationService.Shutdown(); return Task.CompletedTask; }),
            });

            TrayProvider.Initialize(menuItems);
        }

        public void Exit()
        {
            ApplicationService.Shutdown();
        }
    }
}
