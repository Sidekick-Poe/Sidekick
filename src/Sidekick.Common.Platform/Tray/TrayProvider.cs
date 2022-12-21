using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using NotificationIconSharp;

namespace Sidekick.Common.Platform.Tray
{
    public class TrayProvider : IDisposable, ITrayProvider
    {
        private CancellationTokenSource cancellationTokenSource;
        private readonly IOptions<PlatformOptions> options;

        private bool IsInitialized { get; set; }

        private NotificationIcon TrayIcon { get; set; }

        private List<TrayMenuItem> TrayMenuItems { get; set; }

        public TrayProvider(IOptions<PlatformOptions> options)
        {
            cancellationTokenSource = new CancellationTokenSource();
            this.options = options;

            try
            {
                NotificationManager.Initialize("com.sidekick", "Sidekick", options.Value.IconPath);
            }
            catch (Exception ex) { }
        }

        public void Initialize(List<TrayMenuItem> trayMenuItems)
        {
            if (IsInitialized)
            {
                return;
            }

            TrayMenuItems = trayMenuItems;

            TrayIcon = new NotificationIcon(options.Value.IconPath);

            foreach (var menuItem in TrayMenuItems)
            {
                var notificationMenuItem = new NotificationMenuItem(menuItem.Label)
                {
                    Disabled = menuItem.Disabled,
                };
                notificationMenuItem.NotificationMenuItemSelected += NotificationMenuItem_NotificationMenuItemSelected;
                TrayIcon.AddMenuItem(notificationMenuItem);
            }

            Task.Run(StartLoop);

            IsInitialized = true;
        }

        public void StartLoop()
        {
            while (!cancellationTokenSource.IsCancellationRequested)
            {
                TrayIcon?.DoMessageLoop(true);
            }
        }

        private void NotificationMenuItem_NotificationMenuItemSelected(NotificationMenuItem notificationMenuItem)
        {
            var menuItem = TrayMenuItems.FirstOrDefault(x => x.Label == notificationMenuItem.Text);

            if (menuItem != null)
            {
                _ = Task.Run(() => menuItem.OnClick());
            }
        }

        public void SendNotification(string message, string title = null)
        {
            NotificationManager.SendNotification(title, message, Guid.NewGuid().ToString(), options.Value.IconPath);
        }

        public void Dispose()
        {
            cancellationTokenSource.Cancel();
            TrayIcon?.Dispose();
            TrayIcon = null;
        }
    }
}