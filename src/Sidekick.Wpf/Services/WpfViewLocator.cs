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
            var window = Windows.FirstOrDefault(x => x.CurrentWebPath == view.Url);
            if (window == null)
            {
                logger.LogError("Unable to find view {viewUrl}", view.Url);
                return;
            }

            var preferences = await cacheProvider.Get<ViewPreferences>($"view_preference_{view.Key}");

            Application.Current.Dispatcher.Invoke(() =>
            {
                window.Title = view.Title;
                window.MinHeight = view.ViewHeight;
                window.MinWidth = view.ViewWidth;
                window.Height = view.ViewHeight;
                window.Width = view.ViewWidth;

                if (view.ViewType != SidekickViewType.Modal && preferences != null)
                {
                    window.Height = preferences.Height;
                    window.Width = preferences.Width;
                }

                if (view.ViewType == SidekickViewType.Overlay)
                {
                    //window.SetMaximizable(false);
                    //window.SetMinimizable(false);
                    //window.SetSkipTaskbar(true);
                    //window.SetResizable(true);
                    //window.SetAlwaysOnTop(true, OnTopLevel.screenSaver);
                    //window.ShowInactive();
                    //
                    //if (view.CloseOnBlur)
                    //{
                    //    window.Focus();
                    //}
                }
                else if (view.ViewType == SidekickViewType.Modal)
                {
                    //window.SetMaximizable(false);
                    //window.SetMinimizable(false);
                    //window.SetSkipTaskbar(false);
                    //window.SetResizable(false);
                    //window.SetAlwaysOnTop(false);
                    //window.Show();
                }
                else
                {
                    //window.SetMaximizable(true);
                    //window.SetMinimizable(true);
                    //window.SetSkipTaskbar(false);
                    //window.SetResizable(true);
                    //window.SetAlwaysOnTop(false);
                    //window.Show();
                }

                WindowPlacement.ConstrainAndCenterWindowToScreen(window: window);

                if (view.ViewType != SidekickViewType.Modal)
                {
                    //window.OnResize += () => Browser_OnResize(window, view);
                }

                if (view.CloseOnBlur)
                {
                    //window.OnBlur += () => Task.Run(() => Close(view));
                }

                window.Ready();
            });
        }

        /// <inheritdoc/>
        public async Task Maximize(SidekickView view)
        {
            var window = Windows.FirstOrDefault(x => x.CurrentWebPath == view.Url);
            if (window == null)
            {
                logger.LogError("Unable to find view {viewUrl}", view.Url);
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
            var window = Windows.FirstOrDefault(x => x.CurrentWebPath == view.Url);
            if (window == null)
            {
                logger.LogError("Unable to find view {viewUrl}", view.Url);
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
            var window = Windows.FirstOrDefault(x => x.CurrentWebPath == view.Url);
            if (window == null)
            {
                logger.LogError("Unable to find view {viewUrl}", view.Url);
                return Task.CompletedTask;
            }

            Application.Current.Dispatcher.Invoke(() =>
            {
                window.Close();
            });
            Windows.Remove(window);
            return Task.CompletedTask;
        }

        /// <inheritdoc/>
        public async Task CloseAllOverlays()
        {
            // foreach (var overlay in Views.Where(x => x.ViewType == SidekickViewType.Overlay))
            // {
            //     await Close(overlay);
            // }
        }

        /// <inheritdoc/>
        public bool IsOverlayOpened()
        {
            // return Views.Any(x => x.ViewType == SidekickViewType.Overlay);
            return false;
        }

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
    }
}
