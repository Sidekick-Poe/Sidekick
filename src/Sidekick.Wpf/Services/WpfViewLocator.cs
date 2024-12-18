using System.Net;
using System.Windows;
using Microsoft.Extensions.Logging;
using Sidekick.Apis.Poe.CloudFlare;
using Sidekick.Common.Cache;
using Sidekick.Common.Ui.Views;
using Sidekick.Wpf.Helpers;

namespace Sidekick.Wpf.Services
{
    public class WpfViewLocator : IViewLocator
    {
        internal readonly ICacheProvider cacheProvider;
        private readonly ILogger<WpfViewLocator> logger;
        private readonly ICloudflareService cloudflareService;

        internal List<MainWindow> Windows { get; } = new();

        internal string? NextUrl { get; set; }

        public WpfViewLocator(ICacheProvider cacheProvider, ILogger<WpfViewLocator> logger, ICloudflareService cloudflareService)
        {
            this.cacheProvider = cacheProvider;
            this.logger = logger;
            this.cloudflareService = cloudflareService;
            cloudflareService.ChallengeStarted += CloudflareServiceOnChallengeStarted;
        }

        /// <inheritdoc/>
        public async Task Initialize(SidekickView view)
        {
            if (!TryGetWindow(view.CurrentView, out var window))
            {
                return;
            }

            window.SidekickView = view;
            view.CurrentView.ViewChanged += CurrentViewOnViewChanged;
            var preferences = await cacheProvider.Get<ViewPreferences>($"view_preference_{view.CurrentView.Key}");

            Application.Current.Dispatcher.Invoke(() =>
            {
                window.Title = view.CurrentView.Title.StartsWith("Sidekick") ? view.CurrentView.Title.Trim() : $"Sidekick {view.CurrentView.Title}".Trim();
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

        private void CurrentViewOnViewChanged(ICurrentView view)
        {
            if (!TryGetWindow(view, out var window))
            {
                return;
            }

            Application.Current.Dispatcher.Invoke(() =>
            {
                window.Title = $"Sidekick {view.Title}".Trim();
            });
        }

        /// <inheritdoc/>
        public async Task Maximize(SidekickView view)
        {
            if (!TryGetWindow(view.CurrentView, out var window))
            {
                return;
            }

            var preferences = await cacheProvider.Get<ViewPreferences>($"view_preference_{view.CurrentView.Key}");

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
            if (!TryGetWindow(view.CurrentView, out var window))
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
            Application.Current.Dispatcher.Invoke(() =>
            {
                try
                {
                    if (!TryGetWindow(view.CurrentView, out var window))
                    {
                        return;
                    }

                    window.Close();
                    Windows.Remove(window);
                    GC.Collect();
                    GC.WaitForPendingFinalizers();
                    GC.Collect();
                }
                catch (InvalidOperationException ex)
                {
                    logger.LogWarning($"Error Closing Window - {ex.Message}");
                }
            });

            return Task.CompletedTask;
        }

        /// <inheritdoc/>
        public async Task CloseAll()
        {
            foreach (var window in Windows.ToList())
            {
                if (window.SidekickView == null)
                {
                    continue;
                }

                await Close(window.SidekickView);
            }
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
                var window = new MainWindow(this)
                {
                    Topmost = true,
                    ShowInTaskbar = false,
                    ResizeMode = ResizeMode.NoResize,
                };
                Windows.Add(window);
                window.Show();
            });

            return Task.CompletedTask;
        }

        private void CloudflareServiceOnChallengeStarted(Uri uri)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                var window = new CloudflareWindow(logger, cloudflareService, uri)
                {
                    Topmost = true,
                    ShowInTaskbar = false,
                    ResizeMode = ResizeMode.NoResize,
                };
                window.Show();
            });
        }

        private bool TryGetWindow(ICurrentView view, out MainWindow window)
        {
            var windowResult = Windows.FirstOrDefault(x => x.Id == view.Id);

            if (windowResult == null)
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    var viewUrl = WebUtility.UrlDecode(view.Url);
                    windowResult = Windows.FirstOrDefault(x => x.CurrentWebPath == viewUrl)!;
                });
            }

            window = windowResult!;

            if (windowResult != null)
            {
                windowResult.Id = view.Id;
                return true;
            }

            logger.LogError("Unable to find view {viewUrl}", view.Url);
            return false;
        }
    }
}
