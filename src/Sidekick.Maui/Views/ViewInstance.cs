using Sidekick.Common.Blazor.Views;

namespace Sidekick.Maui.Views
{
    /// <inheritdoc/>
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

            return Task.CompletedTask;
        }

        /// <inheritdoc/>
        public Task Close()
        {
            return Task.CompletedTask;
        }
    }
}
