using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Sidekick.Common.Blazor.Layouts.Components;

namespace Sidekick.Common.Blazor.Views
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
        protected IViewLocator ViewLocator { get; set; }

        /// <summary>
        /// Gets or sets the navigation manager.
        /// </summary>
        [Inject]
        public NavigationManager NavigationManager { get; set; }

        /// <summary>
        /// Gets or sets the sidekick wrapper.
        /// </summary>
        [CascadingParameter]
        public SidekickWrapper Wrapper { get; set; }

        /// <summary>
        /// Gets the current url of the view.
        /// </summary>
        public string Url => NavigationManager.Uri.ToString();

        /// <summary>
        /// Gets the key of the view.
        /// </summary>
        public string Key => Url.Split('/', '\\').FirstOrDefault(x => !string.IsNullOrEmpty(x));

        /// <summary>
        /// Gets the title of the view.
        /// </summary>
        public abstract string Title { get; }

        /// <summary>
        /// Gets the type of the view.
        /// </summary>
        public virtual SidekickViewType ViewType => SidekickViewType.Standard;

        /// <summary>
        /// Gets a value indicating whether to close the view when you click outside the window.
        /// </summary>
        public virtual bool CloseOnBlur => false;

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

        /// <summary>
        /// Closes the current view.
        /// </summary>
        /// <returns>A task.</returns>
        public Task Close() => ViewLocator.Close(this);

        /// <inheritdoc/>
        protected override async Task OnInitializedAsync()
        {
            await ViewLocator.Initialize(this);
            await base.OnInitializedAsync();
        }

        protected override void OnAfterRender(bool firstRender)
        {
            if (firstRender && Wrapper != null)
            {
                Wrapper.SetView(this);
            }
            base.OnAfterRender(firstRender);
        }
    }
}
