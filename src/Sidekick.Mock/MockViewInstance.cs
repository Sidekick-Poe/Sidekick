using System;
using System.Threading.Tasks;
using Sidekick.Common.Blazor.Views;

namespace Sidekick.Mock
{
    /// <inheritdoc/>
    public class MockViewInstance : IViewInstance
    {
        /// <inheritdoc/>
        public event Action OnChange;

        /// <inheritdoc/>
        public string Title => string.Empty;

        /// <inheritdoc/>
        public virtual Task Close()
        {
            return Task.CompletedTask;
        }

        /// <inheritdoc/>
        public Task Initialize(string title, int width = 768, int height = 600, bool isOverlay = false, bool isModal = false, bool closeOnBlur = false)
        {
            return Task.CompletedTask;
        }
    }
}
