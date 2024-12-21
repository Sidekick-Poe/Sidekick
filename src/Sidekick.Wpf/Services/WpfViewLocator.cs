using System.Globalization;
using System.Net;
using System.Windows;
using Microsoft.Extensions.Logging;
using Sidekick.Apis.Poe.CloudFlare;
using Sidekick.Common.Settings;
using Sidekick.Common.Ui.Views;
using Sidekick.Wpf.Helpers;

namespace Sidekick.Wpf.Services
{
    public class WpfViewLocator : IViewLocator
    {
        private readonly ILogger<WpfViewLocator> logger;
        private readonly ICloudflareService cloudflareService;
        private readonly ISettingsService settingsService;
        internal readonly IViewPreferenceService ViewPreferenceService;

        internal List<MainWindow> Windows { get; } = new();

        internal string? NextUrl { get; set; }

        public WpfViewLocator(ILogger<WpfViewLocator> logger, ICloudflareService cloudflareService, ISettingsService settingsService, IViewPreferenceService viewPreferenceService)
        {
            this.logger = logger;
            this.cloudflareService = cloudflareService;
            this.settingsService = settingsService;
            this.ViewPreferenceService = viewPreferenceService;
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
            var preferences = await ViewPreferenceService.Get(view.CurrentView.Key);

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
                if (view.Height != null)
                {
                    window.Height = view.Height.Value;
                    CenterHelper.Center(window);
                }

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

            var preferences = await ViewPreferenceService.Get($"view_preference_{view.CurrentView.Key}");

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
                }

                CenterHelper.Center(window);
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
                    CenterHelper.Center(window);
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
        public async Task Open(string url)
        {
            if (string.IsNullOrEmpty(url))
            {
                return;
            }

            NextUrl = url;

            var culture = await settingsService.GetString(SettingKeys.LanguageUi);
            await Application.Current.Dispatcher.InvokeAsync(() =>
            {
                if (string.IsNullOrEmpty(culture))
                {
                    return;
                }

                CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo(culture);
                CultureInfo.CurrentUICulture = CultureInfo.GetCultureInfo(culture);
            });

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
