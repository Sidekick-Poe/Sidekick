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
        private bool disposedValue;
        private CancellationTokenSource cancellationTokenSource;
        private readonly IOptions<PlatformOptions> options;

        private bool IsInitialized { get; set; }

        private NotificationIcon TrayIcon { get; set; }

        private List<TrayMenuItem> TrayMenuItems { get; set; }

        public TrayProvider(IOptions<PlatformOptions> options)
        {
            cancellationTokenSource = new CancellationTokenSource();
            this.options = options;
        }

        public void Initialize(List<TrayMenuItem> trayMenuItems)
        {
            if (IsInitialized)
            {
                return;
            }

            TrayMenuItems = trayMenuItems;

            NotificationManager.Initialize("com.app.sidekick", "App Test", options.Value.IconPath);
            NotificationManager.SendNotification("My New Notification", "Isn't This Handy", "ActionId", options.Value.IconPath);

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

            StartLoop();

            IsInitialized = true;
        }

        private void StartLoop()
        {
            while (!cancellationTokenSource.IsCancellationRequested)
            {
                TrayIcon?.DoMessageLoop(true);
            }

            TrayIcon?.Dispose();
            TrayIcon = null;
        }

        private void NotificationMenuItem_NotificationMenuItemSelected(NotificationMenuItem notificationMenuItem)
        {
            var menuItem = TrayMenuItems.FirstOrDefault(x => x.Label == notificationMenuItem.Text);

            if (menuItem != null)
            {
                _ = Task.Run(() => menuItem.OnClick());
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    cancellationTokenSource.Cancel();
                    cancellationTokenSource.Dispose();
                    // TODO: dispose managed state (managed objects)
                }

                // TODO: free unmanaged resources (unmanaged objects) and override finalizer
                // TODO: set large fields to null
                disposedValue = true;
            }
        }

        // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged
        // resources ~TrayProvider() { // Do not change this code. Put cleanup code in 'Dispose(bool
        // disposing)' method Dispose(disposing: false); }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
