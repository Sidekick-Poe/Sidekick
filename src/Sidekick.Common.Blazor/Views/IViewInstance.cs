using System;
using System.Threading.Tasks;

namespace Sidekick.Common.Blazor.Views
{
    /// <summary>
    /// Interface to manage a view instance
    /// </summary>
    public interface IViewInstance
    {
        /// <summary>
        /// Event that is triggered when the view changes.
        /// </summary>
        event Action OnChange;

        /// <summary>
        ///  Initializes the view
        /// </summary>
        /// <param name="title"></param>
        /// <returns></returns>
        Task Initialize(string title, int width = 768, int height = 600, bool isOverlay = false, bool isModal = false, bool closeOnBlur = false);

        /// <summary>
        /// Minimizes the view
        /// </summary>
        Task Minimize();

        /// <summary>
        /// Maximizes the view
        /// </summary>
        Task Maximize();

        /// <summary>
        /// Closes the view
        /// </summary>
        Task Close();

        /// <summary>
        /// The title of the view
        /// </summary>
        string Title { get; }

        /// <summary>
        /// Gets a value indicating whether the view can be minimized.
        /// </summary>
        bool Minimizable { get; }

        /// <summary>
        /// Gets a value indicating whether the view can be maximized.
        /// </summary>
        bool Maximizable { get; }
    }
}
