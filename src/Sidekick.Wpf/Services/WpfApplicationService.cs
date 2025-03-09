using System.Windows.Controls;
using Hardcodet.Wpf.TaskbarNotification;
using Microsoft.Extensions.Localization;
using Sidekick.Common.Blazor.Home;
using Sidekick.Common.Browser;
using Sidekick.Common.Platform;
using Sidekick.Common.Ui.Views;

namespace Sidekick.Wpf.Services;

public class WpfApplicationService
(
    IViewLocator viewLocator,
    IStringLocalizer<HomeResources> resources,
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
        Icon = new TaskbarIcon();
        Icon.Icon = new System.Drawing.Icon(System.IO.Path.Combine(AppContext.BaseDirectory, "wwwroot/favicon.ico"));
        Icon.ToolTipText = "Sidekick";
        Icon.ContextMenu = new ContextMenu();
        Icon.DoubleClickCommand = new SimpleCommand(() => viewLocator.Open("/home"));

        AddTrayItem("Sidekick - " + ((IApplicationService)this).GetVersion(), null, true);
        AddTrayItem(resources["Home"], () => viewLocator.Open("/home"));
        AddTrayItem(resources["Open_Website"],
                    () =>
                    {
                        browserProvider.OpenSidekickWebsite();
                        return Task.CompletedTask;
                    });
        AddTrayItem(resources["Exit"],
                    () =>
                    {
                        Shutdown();
                        return Task.CompletedTask;
                    });

        Initialized = true;
    }

    private void AddTrayItem(string label, Func<Task>? onClick, bool disabled = false)
    {
        if (Icon?.ContextMenu == null) return;

        var menuItem = new MenuItem
        {
            Header = label,
            IsEnabled = !disabled,
        };

        if (onClick != null)
        {
            menuItem.Click += async (_, _) =>
            {
                await onClick();
            };
        }

        Icon.ContextMenu.Items.Add(menuItem);
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
