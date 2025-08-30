using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using Sidekick.Common;
using Sidekick.Common.Blazor.Initialization;
using Sidekick.Common.Cache;
using Sidekick.Common.Exceptions;
using Sidekick.Common.Initialization;
using Sidekick.Common.Platform;
using Sidekick.Common.Settings;
using Sidekick.Common.Ui.Views;

namespace Sidekick.Modules.General.Initialization;

public partial class Initialization
{
    [Inject]
    private IStringLocalizer<InitializationResources> Resources { get; set; } = null!;

    [Inject]
    private ILogger<Initialization> Logger { get; set; } = null!;

    [Inject]
    private IApplicationService ApplicationService { get; set; } = null!;

    [Inject]
    private IServiceProvider ServiceProvider { get; set; } = null!;

    [Inject]
    private ISettingsService SettingsService { get; set; } = null!;

    [Inject]
    private ICacheProvider CacheProvider { get; set; } = null!;

    [Inject]
    private ICurrentView CurrentView { get; set; } = null!;

    [Inject]
    private NavigationManager NavigationManager { get; set; } = null!;

    private int Count { get; set; }

    private string? Step { get; set; }

    private int Percentage { get; set; }

    public Task? InitializationTask { get; set; }

    private int completed;

    protected override async Task OnInitializedAsync()
    {
        CurrentView.Initialize(new ViewOptions()
        {
            Width = 400,
            Height = 220,
        });
        InitializationTask = Handle();
        await base.OnInitializedAsync();
        await InitializationTask;
    }

    public async Task Handle()
    {
        try
        {
            completed = 0;
            Count = SidekickConfiguration.InitializableServices.Count;
            var version = ApplicationService.GetVersion();
            var previousVersion = await SettingsService.GetString(SettingKeys.Version);
            if (version != previousVersion)
            {
                await CacheProvider.Clear();
                await SettingsService.Set(SettingKeys.Version, version);
            }

            // Report initial progress
            await ReportProgress();

            var services = SidekickConfiguration.InitializableServices
                .Select(serviceType =>
                {
                    var service = ServiceProvider.GetRequiredService(serviceType);
                    return service as IInitializableService;
                })
                .Where(x => x != null)
                .Select(x => x!)
                .GroupBy(s => s.Priority)
                .OrderBy(g => g.Key)
                .ToList();

            foreach (var priorityGroup in services)
            {
                var initializationTasks = priorityGroup.Select(async service =>
                {
                    Logger.LogInformation($"[Initialization] Initializing {service.GetType().FullName}");
                    await service.Initialize();
                    Interlocked.Increment(ref completed);
                    await ReportProgress();
                });
                await Task.WhenAll(initializationTasks);
            }

            // If we have a successful initialization, we delay for half a second to show the
            // "Ready" label on the UI before closing the view
            completed = Count;

            await ReportProgress();
            await Task.Delay(200);
            await Complete();
        }
        catch (SidekickException e)
        {
            await SettingsService.Set(SettingKeys.LanguageParser, null);
            e.Actions = ExceptionActions.ExitApplication;
            throw;
        }
    }

    private async Task Complete()
    {
        var redirectToHome = await SettingsService.GetBool(SettingKeys.OpenHomeOnLaunch);
        if (redirectToHome)
        {
            NavigationManager.NavigateTo("/home");
        }
        else
        {
            CurrentView.Close();
        }
    }

    private Task ReportProgress()
    {
        return InvokeAsync(() =>
        {
            Percentage = Count == 0 ? 0 : completed * 100 / Count;
            if (Percentage >= 100)
            {
                Step = Resources["Ready"];
                Percentage = 100;
            }
            else
            {
                Step = Resources["Title", completed, Count];
            }

            StateHasChanged();
            return Task.Delay(100);
        });
    }

    public void Exit()
    {
        ApplicationService.Shutdown();
    }
}
