using Microsoft.AspNetCore.Components;

namespace Sidekick.Common.Ui.Views
{
    /// <summary>
    /// Base class for sidekick views.
    /// </summary>
    public abstract class SidekickView : ComponentBase
    {
        /// <summary>
        /// Gets or sets the view locator service.
        /// </summary>
        [Inject]
        protected IViewLocator ViewLocator { get; set; } = null!;

        /// <summary>
        /// Gets or sets the view instance service.
        /// </summary>
        [Inject]
        public ICurrentView CurrentView { get; set; } = null!;

        /// <summary>
        /// Gets or sets the navigation manager.
        /// </summary>
        [Inject]
        public NavigationManager NavigationManager { get; set; } = null!;

        /// <summary>
        /// Gets the type of the view.
        /// </summary>
        public virtual SidekickViewType ViewType => SidekickViewType.Standard;

        /// <summary>
        /// Gets a value indicating whether to close the view when you click outside the window.
        /// </summary>
        public virtual bool CloseOnBlur { get; set; } = false;

        /// <summary>
        /// Gets the minimum height of the view.
        /// </summary>
        public virtual int ViewHeight => ViewType switch
        {
            SidekickViewType.Modal => 260,
            _ => 600,
        };

        /// <summary>
        /// Gets the minimum width of the view.
        /// </summary>
        public virtual int ViewWidth => ViewType switch
        {
            SidekickViewType.Modal => 400,
            _ => 768,
        };

        /// <inheritdoc/>
        protected override async Task OnInitializedAsync()
        {
            await CurrentView.Initialize(this);
            await base.OnInitializedAsync();
        }
    }
}
