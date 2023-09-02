using System.Windows;
using System.Windows.Controls;
using Hardcodet.Wpf.TaskbarNotification;
using Sidekick.Common.Blazor.Views;
using Sidekick.Common.Platform;
using Sidekick.Common.Platform.Tray;

namespace Sidekick.Wpf.Services
{
    public class WpfTrayProvider : ITrayProvider, IDisposable
    {
        private readonly IViewLocator viewLocator;

        public WpfTrayProvider(IViewLocator viewLocator)
        {
            this.viewLocator = viewLocator;
        }

        private bool Initialized { get; set; }

        private TaskbarIcon? Icon { get; set; }

        public void Initialize(List<TrayMenuItem> items)
        {
            if (Initialized)
            {
                return;
            }

            Icon = new TaskbarIcon();
            Icon.Icon = new System.Drawing.Icon(@"wwwroot/favicon.ico");
            Icon.ToolTipText = "Sidekick";
            Icon.ContextMenu = new ContextMenu();
            Icon.DoubleClickCommand = new SimpleCommand(() => viewLocator.Open("/settings"));

            foreach (var item in items)
            {
                var menuItem = new MenuItem();
                menuItem.Header = item.Label;
                menuItem.IsEnabled = !item.Disabled;

                menuItem.Click += new RoutedEventHandler(async (sender, args) =>
                {
                    if (item.OnClick != null)
                    {
                        await item.OnClick();
                    }
                });

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
}
