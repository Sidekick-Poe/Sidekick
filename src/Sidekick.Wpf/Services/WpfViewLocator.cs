using System.Net;
using System.Windows;
using Microsoft.Extensions.Logging;
using Sidekick.Common.Blazor.Views;
using Sidekick.Common.Cache;
using Sidekick.Common.Settings;
using Sidekick.Wpf.Helpers;

namespace Sidekick.Wpf.Services
{
    public class WpfViewLocator : IViewLocator
    {
        internal readonly ICacheProvider cacheProvider;
        internal readonly ISettings settings;
        private readonly ILogger<WpfViewLocator> logger;

        public WpfViewLocator(ICacheProvider cacheProvider,
                              ISettings settings,
                              ILogger<WpfViewLocator> logger)
        {
            this.cacheProvider = cacheProvider;
            this.settings = settings;
            this.logger = logger;
        }

        internal List<MainWindow> Windows { get; set; } = new();

        internal string? NextUrl { get; set; }

        /// <inheritdoc/>
        public async Task Initialize(SidekickView view)
        {
            if (!TryGetWindow(view, out var window))
            {
                return;
            }

            window.SidekickView = view;
            var preferences = await cacheProvider.Get<ViewPreferences>($"view_preference_{view.Key}");

            Application.Current.Dispatcher.Invoke(() =>
            {
                window.Title = $"Sidekick {view.Title}";
                window.MinHeight = view.ViewHeight + 20;
                window.MinWidth = view.ViewWidth + 20;

                if (view.ViewType != SidekickViewType.Modal && preferences != null)
                {
                    window.Height = preferences.Height;
                    window.Width = preferences.Width;
                }
                else
                {
                    window.Height = view.ViewHeight + 20;
                    window.Width = view.ViewWidth + 20;
                }

                if (view.ViewType == SidekickViewType.Overlay)
                {
                    window.Topmost = true;
                    window.ShowInTaskbar = false;
                    window.ResizeMode = ResizeMode.CanResize;
                }
                else if (view.ViewType == SidekickViewType.Modal)
                {
                    window.Topmost = true;
                    window.ShowInTaskbar = true;
                    window.ResizeMode = ResizeMode.NoResize;
                }
                else
                {
                    window.Topmost = false;
                    window.ShowInTaskbar = true;
                    window.ResizeMode = ResizeMode.CanResize;
                }

                WindowPlacement.ConstrainAndCenterWindowToScreen(window: window);

                window.Ready();
            });
        }

        /// <inheritdoc/>
        public async Task Maximize(SidekickView view)
        {
            if (!TryGetWindow(view, out var window))
            {
                return;
            }

            var preferences = await cacheProvider.Get<ViewPreferences>($"view_preference_{view.Key}");

            Application.Current.Dispatcher.Invoke(() =>
            {
                if (window.WindowState == WindowState.Normal)
                {
                    window.WindowState = WindowState.Maximized;
                }
                else
                {
                    window.WindowState = WindowState.Normal;

                    if (preferences != null)
                    {
                        window.Height = preferences.Height;
                        window.Width = preferences.Width;
                    }
                    else
                    {
                        window.Height = view.ViewHeight;
                        window.Width = view.ViewWidth;
                    }

                    WindowPlacement.ConstrainAndCenterWindowToScreen(window: window);
                }
            });
        }

        /// <inheritdoc/>
        public Task Minimize(SidekickView view)
        {
            if (!TryGetWindow(view, out var window))
            {
                return Task.CompletedTask;
            }

            Application.Current.Dispatcher.Invoke(() =>
            {
                if (window.WindowState == WindowState.Normal)
                {
                    window.WindowState = WindowState.Minimized;
                }
                else
                {
                    window.WindowState = WindowState.Normal;
                    WindowPlacement.ConstrainAndCenterWindowToScreen(window: window);
                }
            });
            return Task.CompletedTask;
        }

        /// <inheritdoc/>
        public Task Close(SidekickView view)
        {
            if (!TryGetWindow(view, out var window))
            {
                return Task.CompletedTask;
            }

            Application.Current.Dispatcher.Invoke(() =>
            {
                try {
                    window.Close();
                } catch(InvalidOperationException ex) {
                    logger.LogWarning($"Error Closing {window.Name} Window - {ex.Message}");
                }
                
            });
            Windows.Remove(window);
            return Task.CompletedTask;
        }

        /// <inheritdoc/>
        public async Task CloseAllOverlays()
        {
            foreach (var overlay in Windows.Where(x => x.SidekickView?.ViewType == SidekickViewType.Overlay).ToList())
            {
                if (overlay.SidekickView == null)
                {
                    continue;
                }

                await Close(overlay.SidekickView);
            }
        }

        /// <inheritdoc/>
        public bool IsOverlayOpened() => Windows.Any(x => x.SidekickView?.ViewType == SidekickViewType.Overlay);

        /// <inheritdoc/>
        public Task Open(string url)
        {
            if (string.IsNullOrEmpty(url))
            {
                return Task.CompletedTask;
            }

            NextUrl = url;

            Application.Current.Dispatcher.Invoke(() =>
            {
                var window = new MainWindow(this);
                Windows.Add(window);
                window.Show();
            });

            return Task.CompletedTask;
        }

        private bool TryGetWindow(SidekickView view, out MainWindow window)
        {
            MainWindow? windowResult = null;

            Application.Current.Dispatcher.Invoke(() =>
            {
                var viewUrl = WebUtility.UrlDecode(view.Url);
                windowResult = Windows.FirstOrDefault(x => x.CurrentWebPath == viewUrl)!;
            });

            window = windowResult!;
            if (window == null)
            {
                logger.LogError("Unable to find view {viewUrl}", view.Url);
                return false;
            }

            return true;
        }
    }
}
