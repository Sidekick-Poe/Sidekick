using System.Globalization;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Microsoft.Extensions.Localization;
using Sidekick.Common.Blazor.Home;
using Sidekick.Common.Browser;
using Sidekick.Common.Localization;
using Sidekick.Common.Platform;
using Sidekick.Common.Ui.Views;

namespace Sidekick.Avalonia.Services;

public class AvaloniaApplicationService
(
    IViewLocator viewLocator,
    IStringLocalizer<HomeResources> resources,
    IUiLanguageProvider uiLanguageProvider,
    IBrowserProvider browserProvider
) : IApplicationService, IDisposable
{
    private TrayIcon? TrayIcon { get; set; }

    public int Priority => 9000;

    public bool HasInitialized { get; set; }

    public Task Initialize()
    {
        if (HasInitialized) return Task.CompletedTask;

        InitializeTray();
        uiLanguageProvider.OnLanguageChanged += OnLanguageChanged;

        HasInitialized = true;

        return Task.CompletedTask;
    }

    private void OnLanguageChanged(CultureInfo cultureInfo)
    {
        InitializeTray();
    }

    private void InitializeTray()
    {
        TrayIcon?.Dispose();

        var iconPath = Path.Combine(AppContext.BaseDirectory, "wwwroot/favicon.ico");

        TrayIcon = new TrayIcon
        {
            ToolTipText = "Sidekick",
        };

        if (File.Exists(iconPath))
        {
            TrayIcon.Icon = new WindowIcon(iconPath);
        }

        var menu = new NativeMenu();

        var headerItem = new NativeMenuItem("Sidekick - " + ((IApplicationService)this).GetVersion())
        {
            IsEnabled = false,
        };
        menu.Items.Add(headerItem);
        menu.Items.Add(new NativeMenuItemSeparator());

        var homeItem = new NativeMenuItem(resources["Home"]);
        homeItem.Click += (_, _) => viewLocator.Open(SidekickViewType.Standard, "/home");
        menu.Items.Add(homeItem);

        var websiteItem = new NativeMenuItem(resources["Open_Website"]);
        websiteItem.Click += (_, _) => browserProvider.OpenUri(browserProvider.SidekickWebsite);
        menu.Items.Add(websiteItem);

        menu.Items.Add(new NativeMenuItemSeparator());

        var exitItem = new NativeMenuItem(resources["Exit"]);
        exitItem.Click += (_, _) => Shutdown();
        menu.Items.Add(exitItem);

        TrayIcon.Menu = menu;
        TrayIcon.Clicked += (_, _) => viewLocator.Open(SidekickViewType.Standard, "/home");
        TrayIcon.IsVisible = true;
    }

    public void Shutdown()
    {
        if (Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.Shutdown();
        }
    }

    public void Dispose()
    {
        uiLanguageProvider.OnLanguageChanged -= OnLanguageChanged;
        TrayIcon?.Dispose();
    }
}
