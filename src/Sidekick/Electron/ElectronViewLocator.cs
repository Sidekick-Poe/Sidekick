using ElectronNET.API;
using ElectronNET.API.Entities;
using Sidekick.Common.Blazor;
using Sidekick.Common.Blazor.Views;
using Sidekick.Common.Cache;
using Sidekick.Common.Settings;

namespace Sidekick.Electron
{
    public class ElectronViewLocator : IViewLocator
    {
        internal readonly ICacheProvider cacheProvider;
        internal readonly ILogger<ElectronViewLocator> logger;
        internal readonly ISettings settings;
        internal readonly IHostEnvironment hostEnvironment;
        private bool FirstView = true;

        public ElectronViewLocator(ICacheProvider cacheProvider,
                                   ILogger<ElectronViewLocator> logger,
                                   ISettings settings,
                                   IHostEnvironment hostEnvironment)
        {
            this.cacheProvider = cacheProvider;
            this.logger = logger;
            this.settings = settings;
            this.hostEnvironment = hostEnvironment;

            ElectronNET.API.Electron.IpcMain.OnSync("close", (viewName) =>
            {
                logger.LogError("Force closing the Blazor view {viewName}", viewName);
                // _ = TryCloseViews((_) => true);
                return null;
            });
        }

        private List<BrowserWindow> Windows { get; set; } = new();

        public void Add(SidekickView view)
        {
            throw new NotImplementedException();
        }

        public void Close(SidekickView view)
        {
            throw new NotImplementedException();
        }

        public void CloseAllOverlays()
        {
            throw new NotImplementedException();
        }

        public bool IsOverlayOpened()
        {
            throw new NotImplementedException();
        }

        public async Task Open(string url)
        {
            if (string.IsNullOrEmpty(url))
            {
                return;
            }

            var window = await CreateBrowser(url);
            Windows.Add(window);
            logger.LogError(await window.WebContents.GetUrl());
        }

        public void Remove(SidekickView view)
        {
            throw new NotImplementedException();
        }

        private async Task<BrowserWindow> CreateBrowser(string path)
        {
            var window = await ElectronNET.API.Electron.WindowManager.CreateWindowAsync(new BrowserWindowOptions
            {
                AcceptFirstMouse = true,
                Center = true,
                Frame = false,
                Fullscreenable = false,
                HasShadow = false,
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

            window.OnReadyToShow += () => window.Show();
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
    }
}
