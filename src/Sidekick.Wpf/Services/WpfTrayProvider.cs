using System.Diagnostics;
using System.Windows.Controls;
using Hardcodet.Wpf.TaskbarNotification;
using Sidekick.Common.Platform;
using Sidekick.Common.Ui.Views;

namespace Sidekick.Wpf.Services;

public class WpfTrayProvider(IViewLocator viewLocator) : ITrayProvider, IDisposable
{
    private bool Initialized { get; set; }

    private TaskbarIcon? Icon { get; set; }

    public void Initialize(List<TrayMenuItem> items)
    {
        if (Initialized)
        {
            return;
        }

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

    public void Dispose()
    {
        if (Icon != null)
        {
            Icon.Dispose();
        }
    }
}
