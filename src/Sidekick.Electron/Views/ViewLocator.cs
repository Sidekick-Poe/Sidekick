using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ElectronNET.API;
using ElectronNET.API.Entities;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Sidekick.Common.Blazor.Views;
using Sidekick.Common.Cache;
using Sidekick.Common.Settings;

namespace Sidekick.Electron.Views
{
    public class ViewLocator : IViewLocator
    {
        internal readonly ICacheProvider cacheProvider;
        internal readonly ILogger<ViewLocator> logger;
        internal readonly ISettings settings;
        internal readonly ElectronCookieProtection electronCookieProtection;
        internal readonly IHostEnvironment hostEnvironment;

        public ViewLocator(ICacheProvider cacheProvider,
                           ILogger<ViewLocator> logger,
                           ISettings settings,
                           ElectronCookieProtection electronCookieProtection,
                           IHostEnvironment hostEnvironment)
        {
            this.cacheProvider = cacheProvider;
            this.logger = logger;
            this.settings = settings;
            this.electronCookieProtection = electronCookieProtection;
            this.hostEnvironment = hostEnvironment;

            ElectronNET.API.Electron.IpcMain.OnSync("close", (viewName) =>
            {
                logger.LogError("Force closing the Blazor view {viewName}", viewName);
                _ = TryCloseViews((_) => true);

                return null;
            });
        }

        private bool FirstView = true;

        internal List<InternalViewInstance> Views { get; set; } = new List<InternalViewInstance>();

        private async Task<BrowserWindow> CreateBrowser(string path)
        {
            var window = await ElectronNET.API.Electron.WindowManager.CreateWindowAsync(new BrowserWindowOptions
            {
                AcceptFirstMouse = true,
                Center = true,
                Frame = false,
                Fullscreenable = false,
                HasShadow = true,
                Show = false,
                Transparent = true,
                DarkTheme = true,
                EnableLargerThanScreen = false,
                WebPreferences = new WebPreferences()
                {
                    NodeIntegration = true,
                }
            }, $"http://localhost:{BridgeSettings.WebPort}{path}");

            window.WebContents.OnCrashed += (killed) =>
            {
                logger.LogWarning("The view has crashed. Attempting to close the window.");
                window.Close();
            };

            window.OnUnresponsive += () =>
            {
                logger.LogWarning("The view has become unresponsive. Attempting to close the window.");
                window.Close();
            };

            await window.WebContents.Session.Cookies.SetAsync(electronCookieProtection.Cookie);

            // Make sure the title is always Sidekick. For keybind management we are watching for this value.
            window.SetTitle("Sidekick");
            window.SetVisibleOnAllWorkspaces(true);

            if (FirstView)
            {
                FirstView = false;
                await window.WebContents.Session.ClearCacheAsync();
            }

            return window;
        }

        public async Task Open(string url)
        {
            if (string.IsNullOrEmpty(url))
            {
                return;
            }

            var newView = new InternalViewInstance(await CreateBrowser(url), url);

            await TryCloseViews(x => x.Key == newView.Key);

            Views.Add(newView);
        }

        public bool IsOverlayOpened()
        {
            return Views.Any(x => x.IsOverlay);
        }

        public void CloseAllOverlays()
        {
            _ = TryCloseViews((view) => view.IsOverlay);
        }

        private async Task TryCloseViews(Func<InternalViewInstance, bool> func)
        {
            while (Views.Any(func))
            {
                var view = Views.FirstOrDefault(func);

                try
                {
                    if (!await view.Browser.IsDestroyedAsync())
                    {
                        view.Browser.Close();
                    }
                }
                catch (Exception)
                {
                }

                Views.Remove(view);
            }
        }
    }
}
