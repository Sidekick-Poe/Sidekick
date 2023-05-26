using System.Collections.Generic;
using System.Threading.Tasks;

namespace Sidekick.Common.Blazor.Views
{
    /// <summary>
    /// Interface to manage views
    /// </summary>
    public interface IViewLocator
    {
        /// <summary>
        /// Gets a list of currently opened views.
        /// </summary>
        List<SidekickView> Views { get; }

        /// <summary>
        /// Opens the specified view
        /// </summary>
        /// <param name="url">The url of the page to load and show</param>
        Task Open(string url);

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
