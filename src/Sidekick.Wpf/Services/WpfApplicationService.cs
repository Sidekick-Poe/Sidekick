using System.Globalization;
using System.IO;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using NotificationIcon.NET;
using Sidekick.Common.Blazor.Home;
using Sidekick.Common.Browser;
using Sidekick.Common.Localization;
using Sidekick.Common.Platform;
using Sidekick.Common.Ui.Views;

namespace Sidekick.Wpf.Services;

public class WpfApplicationService
(
    IViewLocator viewLocator,
    ILogger<WpfApplicationService> logger,
    IStringLocalizer<HomeResources> resources,
    IUiLanguageProvider uiLanguageProvider,
    IBrowserProvider browserProvider
) : IApplicationService, IDisposable
{
    public bool SupportsKeybinds => true;

    private bool Initialized { get; set; }

    private NotifyIcon? Icon { get; set; }

    public int Priority => 9000;

    public Task Initialize()
    {
        if (Initialized)
        {
            return Task.CompletedTask;
        }
        InitializeTray();

        uiLanguageProvider.OnLanguageChanged += OnLanguageChanged;

        Initialized = true;

        return Task.CompletedTask;
    }

    private void OnLanguageChanged(CultureInfo cultureInfo)
    {
        if (Icon is null)
        {
            return;
        }

        Icon.MenuItems[0].Text = resources["Sidekick"] + " - " + ((IApplicationService)this).GetVersion();
        Icon.MenuItems[1].Text = resources["Home"];
        Icon.MenuItems[2].Text = resources["Open_Website"];
        Icon.MenuItems[3].Text = resources["Exit"];
    }

    private void InitializeTray()
    {
        var iconImage = Path.Combine(AppContext.BaseDirectory, "wwwroot/favicon.ico");

        Icon = NotifyIcon.Create(iconImage,
        [
            new("Sidekick - " + ((IApplicationService)this).GetVersion())
            {
                IsDisabled = true
            },
            new(resources["Home"])
            {
                Click = (s, e) => viewLocator.Open(SidekickViewType.Standard, "/home")
            },
            new(resources["Open_Website"])
            {
                Click = (s, e) => browserProvider.OpenUri(browserProvider.SidekickWebsite)
            },
            new(resources["Exit"])
            {
                Click = (s, e) => Shutdown()
            },
        ]);

        // Runs its own message loop.
        _ = Task.Run(() =>
        {
            try
            {
                Icon.Show();
            }
            catch (Exception e)
            {
                logger.LogError(e, "[ApplicationService] Error while trying to create the notification icon in the taskbar.");
            }
        });
    }

    public void Shutdown()
    {
        System.Windows.Application.Current.Dispatcher.Invoke(() =>
        {
            System.Windows.Application.Current.Shutdown();
        });
        Environment.Exit(0);
    }

    public void Dispose()
    {
        uiLanguageProvider.OnLanguageChanged -= OnLanguageChanged;
        Icon?.Dispose();
    }
}
