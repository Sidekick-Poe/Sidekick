using System.Threading.Tasks;

namespace Sidekick.Common.Blazor.Views
{
    /// <summary>
    /// Interface to manage views
    /// </summary>
    public interface IViewLocator
    {
        /// <summary>
        /// Gets a value indicating whether the application is currently running inside Electron.
        /// </summary>
        bool IsElectron { get; }

        /// <summary>
        /// Opens the specified view
        /// </summary>
        /// <param name="url">The url of the page to load and show</param>
        Task Open(string url);

        /// <summary>
        /// Initializes a view that was previously opened.
        /// </summary>
        /// <param name="view">The view to initialize.</param>
        Task Initialize(SidekickView view);

        /// <summary>
        /// Minimizes the specified view.
        /// </summary>
        /// <param name="view">The view to minimize.</param>
        Task Minimize(SidekickView view);

        /// <summary>
        /// Maximizes the specified view.
        /// </summary>
        /// <param name="view">The view to maximize.</param>
        Task Maximize(SidekickView view);

        /// <summary>
        /// Closes the specified view.
        /// </summary>
        /// <param name="view">The view to close.</param>
        Task Close(SidekickView view);

        /// <summary>
        /// Close all overlays
        /// </summary>
        Task CloseAllOverlays();

        /// <summary>
        /// Check if an overlay is opened
        /// </summary>
        /// <returns>true if a view is opened. false otherwise</returns>
        bool IsOverlayOpened();
    }
}
