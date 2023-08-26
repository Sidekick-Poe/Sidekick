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

        private List<MainWindow> Windows { get; set; } = new();

        internal string? NextUrl { get; set; }

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

        public async Task Maximize(SidekickView view)
        {
            var window = Windows.FirstOrDefault(x => x.CurrentWebPath == view.Url);
            if (window == null)
            {
                logger.LogError("Unable to find view {viewUrl}", view.Url);
                return;
            }

            // if (!await browser.IsMaximizedAsync())
            // {
            //     browser.Maximize();
            // }
            // else
            // {
            //     var preferences = await cacheProvider.Get<ViewPreferences>($"view_preference_{view.Key}");
            //     if (preferences != null)
            //     {
            //         browser.SetSize(preferences.Width, preferences.Height);
            //     }
            //     else
            //     {
            //         browser.SetSize(view.ViewWidth, view.ViewHeight);
            //     }
            //
            //     browser.Center();
            // }
        }

        public Task Minimize(SidekickView view)
        {
            var window = Windows.FirstOrDefault(x => x.CurrentWebPath == view.Url);
            if (window == null)
            {
                logger.LogError("Unable to find view {viewUrl}", view.Url);
                return Task.CompletedTask;
            }

            // browser.Minimize();
            return Task.CompletedTask;
        }

        public async Task Close(SidekickView view)
        {
            var window = Windows.FirstOrDefault(x => x.CurrentWebPath == view.Url);
            if (window == null)
            {
                logger.LogError("Unable to find view {viewUrl}", view.Url);
                return;
            }

            // if (!await browser.IsDestroyedAsync())
            // {
            //     browser.Close();
            // }
            //
            // // Todo remove events
            //
            // Views.Remove(view);
            // Windows.Remove(browser);
        }

        public async Task CloseAllOverlays()
        {
            // foreach (var overlay in Views.Where(x => x.ViewType == SidekickViewType.Overlay))
            // {
            //     await Close(overlay);
            // }
        }

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
                var window = new MainWindow();
                Windows.Add(window);
                window.Show();
            });

            return Task.CompletedTask;
        }
    }
}
