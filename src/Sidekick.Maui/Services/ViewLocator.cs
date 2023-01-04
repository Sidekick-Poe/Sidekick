using Sidekick.Common.Blazor.Views;

namespace Sidekick.Maui.Services
{
    /// Implementation of the IViewLocator interface.
    public class ViewLocator : IViewLocator
    {
        private readonly IServiceProvider serviceProvider;

        public ViewLocator(IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider;
        }

        public void CloseAllOverlays()
        {
        }

        public bool IsOverlayOpened()
        {
            return false;
        }

        public Task Open(string url)
        {
            Application.Current.Dispatcher.Dispatch(() =>
            {
                Application.Current.OpenWindow(new Window(new BlazorWindow(serviceProvider, url)));
            });
            return Task.CompletedTask;
        }

        #region Maui workings

        /// <summary>
        /// Gets the start page of the next window to be opened.
        /// </summary>
        internal string NextWindowUrl { get; private set; }

        /// <summary>
        /// Gets the window object of the next window that is being opened.
        /// </summary>
        internal BlazorWindow NextWindow { get; private set; }

        internal void SetNextWindow(BlazorWindow window, string initialUrl)
        {
            NextWindow = window;
            NextWindowUrl = initialUrl;
        }

        internal void ClearNextWindow()
        {
            NextWindow = null;
            NextWindowUrl = null;
        }

        #endregion Maui workings
    }
}
