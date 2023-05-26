using System.Threading.Tasks;

namespace Sidekick.Common.Blazor.Views
{
    /// <summary>
    /// Interface to manage views
    /// </summary>
    public interface IViewLocator
    {
        /// <summary>
        /// Opens the specified view
        /// </summary>
        /// <param name="url">The url of the page to load and show</param>
        Task Open(string url);

        /// <summary>
        /// Closes the specified view.
        /// </summary>
        /// <param name="view">The view to close.</param>
        void Close(SidekickView view);

        /// <summary>
        /// Adds a sidekick view to the active views.
        /// </summary>
        /// <param name="view">The view to add.</param>
        void Add(SidekickView view);

        /// <summary>
        /// Removes a sidekick view from being active.
        /// </summary>
        /// <param name="view">The view to remove.</param>
        void Remove(SidekickView view);

        /// <summary>
        /// Close all overlays
        /// </summary>
        void CloseAllOverlays();

        /// <summary>
        /// Check if an overlay is opened
        /// </summary>
        /// <returns>true if a view is opened. false otherwise</returns>
        bool IsOverlayOpened();
    }
}
