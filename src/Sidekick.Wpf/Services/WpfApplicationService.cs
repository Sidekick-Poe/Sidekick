using System.Diagnostics;
using System.Windows.Controls;
using Hardcodet.Wpf.TaskbarNotification;
using Microsoft.Extensions.Localization;
using Sidekick.Common.Blazor.Initialization;
using Sidekick.Common.Browser;
using Sidekick.Common.Platform;
using Sidekick.Common.Ui.Views;

namespace Sidekick.Wpf.Services;

public class WpfApplicationService(
    IViewLocator viewLocator,
    IStringLocalizer<InitializationResources> resources,
    IBrowserProvider browserProvider
) : IApplicationService, IDisposable
{
    public bool SupportsKeybinds => true;

    private bool Initialized { get; set; }

    private TaskbarIcon? Icon { get; set; }

    public int Priority => 9000;

    public Task Initialize()
    {
        if (Initialized)
        {
            return Task.CompletedTask;
        }

        InitializeTray();
        return Task.CompletedTask;
    }

    private void InitializeTray()
    {
        var items = new List<TrayMenuItem>();

        items.AddRange(new List<TrayMenuItem>()
        {
            new(label: "Sidekick - " + ((IApplicationService)this).GetVersion()),
            new(label: resources["Open_Website"],
                onClick: () =>
                {
                    browserProvider.OpenSidekickWebsite();
                    return Task.CompletedTask;
                }),

            // new(label: "Wealth", onClick: () => ViewLocator.Open("/wealth")),

            new(label: resources["Settings"], onClick: () => viewLocator.Open("/settings")),
            new(label: resources["Exit"],
                onClick: () =>
                {
                    Shutdown();
                    return Task.CompletedTask;
                }),
        });

        Icon = new TaskbarIcon();
        Icon.Icon = new System.Drawing.Icon(System.IO.Path.Combine(AppContext.BaseDirectory, "wwwroot/favicon.ico"));
        Icon.ToolTipText = "Sidekick";
        Icon.ContextMenu = new ContextMenu();
        Icon.DoubleClickCommand = new SimpleCommand(() => viewLocator.Open("/settings"));

        if (Debugger.IsAttached)
        {
            var developmentMenuItem = new MenuItem
            {
                Header = "Development",
                IsEnabled = true,
            };

            developmentMenuItem.Click += async (_, _) =>
            {
                await viewLocator.Open("/development");
            };

            Icon.ContextMenu.Items.Add(developmentMenuItem);
        }

        foreach (var item in items)
        {
            var menuItem = new MenuItem
            {
                Header = item.Label,
                IsEnabled = !item.Disabled,
            };

            menuItem.Click += async (_, _) =>
            {
                if (item.OnClick != null)
                {
                    await item.OnClick();
                }
            };

            Icon.ContextMenu.Items.Add(menuItem);
        }

        Initialized = true;
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
        if (Icon != null)
        {
            Icon.Dispose();
        }
    }
}
