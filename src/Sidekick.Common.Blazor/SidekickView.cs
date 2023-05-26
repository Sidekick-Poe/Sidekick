using System;
using Microsoft.AspNetCore.Components;
using Sidekick.Common.Blazor.Views;

namespace Sidekick.Common.Blazor
{
    /// <summary>
    /// Base class for sidekick views.
    /// </summary>
    public abstract class SidekickView : ComponentBase, IDisposable
    {
        [Inject]
        private IViewLocator ViewLocator { get; set; }

        [Inject]
        private NavigationManager NavigationManager { get; set; }

        /// <summary>
        /// Event that is triggered when the view changes.
        /// </summary>
        public event Action OnChange;

        /// <summary>
        /// Gets the current url of the view.
        /// </summary>
        public string Url => NavigationManager.Uri.ToString();

        /// <summary>
        /// Gets the title of the view.
        /// </summary>
        public abstract string Title { get; }

        /// <summary>
        /// Gets the type of the view.
        /// </summary>
        public abstract SidekickViewType Type { get; }

        /// <summary>
        /// Gets a value indicating whether the view can be minimized.
        /// </summary>
        public virtual bool Minimizable => Type == SidekickViewType.Standard;

        /// <summary>
        /// Gets a value indicating whether the view can be maximized.
        /// </summary>
        public virtual bool Maximizable => Type == SidekickViewType.Standard;

        /// <summary>
        /// Gets the minimum height of the view.
        /// </summary>
        public virtual int ViewHeight => Type switch
        {
            _ => 600,
        };

        /// <summary>
        /// Gets the minimum width of the view.
        /// </summary>
        public virtual int ViewWidth => Type switch
        {
            _ => 768,
        };

        /// <inheritdoc/>
        protected override void OnInitialized()
        {
            ViewLocator.Add(this);
            base.OnInitialized();
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            ViewLocator.Remove(this);
        }
    }
}
