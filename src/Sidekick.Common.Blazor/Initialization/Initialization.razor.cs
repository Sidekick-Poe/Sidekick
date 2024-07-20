using System.Diagnostics;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Sidekick.Common.Browser;
using Sidekick.Common.Initialization;
using Sidekick.Common.Keybinds;
using Sidekick.Common.Platform;
using Sidekick.Common.Settings;
using Sidekick.Common.Ui.Views;

namespace Sidekick.Common.Blazor.Initialization
{
    public partial class Initialization : SidekickView
    {
        [Inject]
        private InitializationResources Resources { get; set; } = null!;

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

        [Inject]
        private ISettingsService SettingsService { get; set; } = null!;

        private int Count { get; set; }

        private int Completed { get; set; }

        private string? Step { get; set; }

        private int Percentage { get; set; }

        private bool Error { get; set; }

        private string? WelcomeMessage { get; set; }

        public Task? InitializationTask { get; set; }

        public override SidekickViewType ViewType => SidekickViewType.Modal;

        public override int ViewHeight => 210;

        protected override async Task OnInitializedAsync()
        {
            InitializationTask = Handle();
            var keyOpenPriceCheck = await SettingsService.GetString(SettingKeys.KeyOpenPriceCheck);
            var keyClose = await SettingsService.GetString(SettingKeys.KeyClose);
            WelcomeMessage = string.Format(Resources.Notification, keyOpenPriceCheck.ToKeybindString(), keyClose.ToKeybindString());
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

                    Logger.LogInformation($"[Initialization] Initializing {initializableService.GetType().FullName}");
                    await Run(initializableService.Initialize);
                }

                await Run(InitializeTray);

                // If we have a successful initialization, we delay for half a second to show the
                // "Ready" label on the UI before closing the view
                Completed = Count;
                await ReportProgress();

                if (Debugger.IsAttached)
                {
                    await ViewLocator.Open("/development");
                    await CurrentView.Close();
                }
                else
                {
                    await Task.Delay(4000);
                    await CurrentView.Close();
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex.Message, "[Initialization] An initialization step failed.");
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
            return InvokeAsync(
                () =>
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

            menuItems.AddRange(
                new List<TrayMenuItem>()
                {
                    new(
                        label: "Sidekick - "
                               + FileVersionInfo.GetVersionInfo(
                                                    GetType()
                                                        .Assembly.Location)
                                                .ProductVersion),
                    new(
                        label: "Open Website",
                        onClick: () =>
                        {
                            BrowserProvider.OpenSidekickWebsite();
                            return Task.CompletedTask;
                        }),

                    // new(label: "Wealth", onClick: () => ViewLocator.Open("/wealth")),

                    new(label: "Settings", onClick: () => ViewLocator.Open("/settings")),
                    new(
                        label: "Exit",
                        onClick: () =>
                        {
                            ApplicationService.Shutdown();
                            return Task.CompletedTask;
                        }),
                });

            TrayProvider.Initialize(menuItems);
        }

        public void Exit()
        {
            ApplicationService.Shutdown();
        }
    }
}
