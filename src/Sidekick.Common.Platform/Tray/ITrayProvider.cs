using System;
using System.Collections.Generic;

namespace Sidekick.Common.Platform.Tray
{
    /// <summary>
    /// Interface containing functions pertaining to tray features
    /// </summary>
    public interface ITrayProvider : IDisposable
    {
        /// <summary>
        /// Initializes the tray provider. Creates the tray icon with the menu as specified in the PlatformOptions.
        /// </summary>
        /// <param name="trayMenuItems">Contains the list of menu items to show in the tray menu.</param>
        void Initialize(List<TrayMenuItem> trayMenuItems);

        /// <summary>
        /// Sends a system notification.
        /// </summary>
        /// <param name="message">Notification message.</param>
        /// <param name="title">Notification title (optional).</param>/
        void SendNotification(string message, string title = null);
    }
}
