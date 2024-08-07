using ElectronNET.API;
using ElectronNET.API.Entities;
using Sidekick.Common.Cache;
using Sidekick.Common.Ui.Views;

namespace Sidekick.Electron
{
    public class ElectronViewLocator : IViewLocator
    {
        internal readonly ICacheProvider CacheProvider;
        internal readonly ILogger<ElectronViewLocator> Logger;
        private bool firstView = true;

        public ElectronViewLocator(ICacheProvider cacheProvider,
                                   ILogger<ElectronViewLocator> logger)
        {
            CacheProvider = cacheProvider;
            Logger = logger;

            ElectronNET.API.Electron.IpcMain.OnSync("close", (viewName) =>
            {
                logger.LogError("Force closing the Blazor view {viewName}", viewName);
                // _ = TryCloseViews((_) => true);
                return null;
            });
        }

        private List<BrowserWindow> Browsers { get; } = new();

        private List<SidekickView> Views { get; } = new();

        public async Task Initialize(SidekickView view)
        {
            var browser = Browsers.FirstOrDefault(x => x.WebContents.GetUrl().Result == view.CurrentView.Url);
            if (browser == null)
            {
                Logger.LogError("Unable to find view {viewUrl}", view.CurrentView.Url);
                return;
            }

            Views.Add(view);

            browser.SetTitle("Sidekick");
            browser.SetMinimumSize(view.ViewWidth, view.ViewHeight);
            browser.SetSize(view.ViewWidth, view.ViewHeight);

            var preferences = await CacheProvider.Get<ViewPreferences>($"view_preference_{view.CurrentView.Key}");
            if (view.ViewType != SidekickViewType.Modal && preferences != null)
            {
                browser.SetSize(preferences.Width, preferences.Height);
            }

            if (view.ViewType == SidekickViewType.Overlay)
            {
                browser.SetMaximizable(false);
                browser.SetMinimizable(false);
                browser.SetSkipTaskbar(true);
                browser.SetResizable(true);
                browser.SetAlwaysOnTop(true, OnTopLevel.screenSaver);
                browser.ShowInactive();

                if (view.CloseOnBlur)
                {
                    browser.Focus();
                }
            }
            else if (view.ViewType == SidekickViewType.Modal)
            {
                browser.SetMaximizable(false);
                browser.SetMinimizable(false);
                browser.SetSkipTaskbar(false);
                browser.SetResizable(false);
                browser.SetAlwaysOnTop(false);
                browser.Show();
            }
            else
            {
                browser.SetMaximizable(true);
                browser.SetMinimizable(true);
                browser.SetSkipTaskbar(false);
                browser.SetResizable(true);
                browser.SetAlwaysOnTop(false);
                browser.Show();
            }

            browser.Center();

            if (view.ViewType != SidekickViewType.Modal)
            {
                browser.OnResize += () => Browser_OnResize(browser, view);
            }

            if (view.CloseOnBlur)
            {
                browser.OnBlur += () => Task.Run(() => Close(view));
            }

            browser.OnClose += () => Browser_OnClose(browser, view);
        }

        public async Task Maximize(SidekickView view)
        {
            var browser = Browsers.FirstOrDefault(x => x.WebContents.GetUrl().Result == view.CurrentView.Url);
            if (browser == null)
            {
                return;
            }

            if (!await browser.IsMaximizedAsync())
            {
                browser.Maximize();
            }
            else
            {
                var preferences = await CacheProvider.Get<ViewPreferences>($"view_preference_{view.CurrentView.Key}");
                if (preferences != null)
                {
                    browser.SetSize(preferences.Width, preferences.Height);
                }
                else
                {
                    browser.SetSize(view.ViewWidth, view.ViewHeight);
                }

                browser.Center();
            }
        }

        public Task Minimize(SidekickView view)
        {
            var browser = Browsers.FirstOrDefault(x => x.WebContents.GetUrl().Result == view.CurrentView.Url);
            if (browser == null)
            {
                return Task.CompletedTask;
            }

            browser.Minimize();
            return Task.CompletedTask;
        }

        public async Task Close(SidekickView view)
        {
            var browser = Browsers.FirstOrDefault(x => x.WebContents.GetUrl().Result == view.CurrentView.Url);
            if (browser == null)
            {
                return;
            }

            if (!await browser.IsDestroyedAsync())
            {
                browser.Close();
            }

            Views.Remove(view);
            Browsers.Remove(browser);
        }

        public async Task CloseAll()
        {
            foreach (var view in Views.ToList())
            {
                await Close(view);
            }
        }

        public async Task CloseAllOverlays()
        {
            foreach (var overlay in Views.Where(x => x.ViewType == SidekickViewType.Overlay).ToList())
            {
                await Close(overlay);
            }
        }

        public bool IsOverlayOpened()
        {
            return Views.Any(x => x.ViewType == SidekickViewType.Overlay);
        }

        public async Task Open(string url)
        {
            if (string.IsNullOrEmpty(url))
            {
                return;
            }

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
                ThickFrame = false,
                TitleBarStyle = TitleBarStyle.hidden,
                UseContentSize = false,
                EnableLargerThanScreen = false,
                WebPreferences = new WebPreferences()
                {
                    NodeIntegration = false,
                }
            }, $"http://localhost:{BridgeSettings.WebPort}{url}");

            window.WebContents.OnCrashed += (_) =>
            {
                Logger.LogWarning("The view has crashed. Attempting to close the window.");
                window.Close();
            };

            window.OnUnresponsive += () =>
            {
                Logger.LogWarning("The view has become unresponsive. Attempting to close the window.");
                window.Close();
            };

            window.OnReadyToShow += () => window.Show();
            // Make sure the title is always Sidekick. For keybind management we are watching for this value.
            window.SetTitle("Sidekick");
            window.SetVisibleOnAllWorkspaces(true);

            if (firstView)
            {
                firstView = false;
                await window.WebContents.Session.ClearCacheAsync();
            }

#if DEBUG
            window.WebContents.OpenDevTools();
#endif

            Browsers.Add(window);
        }

        private ulong resizeBounce;

        private void Browser_OnResize(BrowserWindow browser, SidekickView view)
        {
            var currentBounce = ++resizeBounce;
            Task.Run(async () =>
            {
                try
                {
                    await Task.Delay(500);
                    if (currentBounce == resizeBounce)
                    {
                        if (!await browser.IsMaximizedAsync())
                        {
                            var bounds = await browser.GetBoundsAsync();
                            await CacheProvider.Set(
                                $"view_preference_{view.CurrentView.Key}",
                                new ViewPreferences()
                                {
                                    Width = bounds.Width,
                                    Height = bounds.Height
                                });
                        }
                    }
                }
                catch (Exception)
                {
                    // Do not stop execution if something goes wrong here.
                }
            });
        }

        private void Browser_OnClose(BrowserWindow browser, SidekickView view)
        {
            Browsers.Remove(browser);
            Views.Remove(view);
        }
    }
}
