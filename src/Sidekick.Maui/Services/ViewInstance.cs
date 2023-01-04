using Sidekick.Common.Blazor.Views;

namespace Sidekick.Maui.Services
{
    /// Implementation of the IViewInstance interface.
    public class ViewInstance : IViewInstance
    {
        /// <inheritdoc/>
        public event Action OnChange;

        /// <inheritdoc/>
        public string Title { get; private set; }

        /// <inheritdoc/>
        public Task Initialize(string title, int width = 768, int height = 600, bool isOverlay = false, bool isModal = false, bool closeOnBlur = false)
        {
            Title = title;

            InitWindow();

            return Task.CompletedTask;
        }

        /// <inheritdoc/>
        public Task Close()
        {
            return Task.CompletedTask;
        }

        #region Maui workings

        internal BlazorWindow Window { get; set; }

        /// <summary>
        /// Sets the MauiPage object related to this view.
        /// </summary>
        /// <param name="window">The window object.</param>
        internal void SetWindow(BlazorWindow window)
        {
            Window = window;

            InitWindow();
        }

        /// <summary>
        /// Sets the MauiPage object related to this view.
        /// </summary>
        /// <param name="window">The window object.</param>
        internal void InitWindow()
        {
            if (Window != null)
            {
                Window.Title = Title;
            }
        }

        #endregion Maui workings
    }
}
