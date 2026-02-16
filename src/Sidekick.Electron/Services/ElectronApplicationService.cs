using ElectronNET.API.Entities;
using Microsoft.Extensions.Localization;
using Sidekick.Common.Blazor.Home;
using Sidekick.Common.Browser;
using Sidekick.Common.Localization;
using Sidekick.Common.Platform;
using Sidekick.Common.Ui.Views;
using System.Drawing;
using System.Globalization;
using static ElectronNET.API.Electron;

namespace Sidekick.Electron.Services;

public class ElectronApplicationService
(
    IViewLocator viewLocator,
    IStringLocalizer<HomeResources> resources,
    IBrowserProvider browserProvider,
     IUiLanguageProvider uiLanguageProvider
) : IApplicationService
{
    public bool SupportsKeybinds => true;

    public bool SupportsAuthentication => true;

    public bool SupportsHardwareAcceleration => true;

    private bool Initialized { get; set; }

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
        Tray.SetMenuItems(GetTrayMenuItems());
    }

    private async void InitializeTray()
    {
        var iconImage = Path.Combine(AppContext.BaseDirectory, "wwwroot/favicon.ico");

        await Tray.Show(iconImage, GetTrayMenuItems());

        Tray.OnClick += (_, _) =>
        {
            WindowManager.BrowserWindows.First().Show();
            viewLocator.Open(SidekickViewType.Standard, "/home");
        };

        await Tray.SetToolTip("Sidekick");
    }

    private MenuItem[] GetTrayMenuItems()
    {
        return
        [
            new () { Label = "Sidekick - " + ((IApplicationService)this).GetVersion(), Enabled = false },
            new () { Type = MenuType.separator },
            new () { Label = resources["Home"], Click = () => viewLocator.Open(SidekickViewType.Standard, "/home") },
            new () { Label = resources["Open_Website"], Click = () => browserProvider.OpenUri(browserProvider.SidekickWebsite) },
            new () { Label = resources["Exit"], Click = () => Shutdown() }
        ];
    }

    public void Shutdown()
    {
        App.Quit();
    }

    public void Dispose()
    {
        uiLanguageProvider.OnLanguageChanged -= OnLanguageChanged;
    }
}
