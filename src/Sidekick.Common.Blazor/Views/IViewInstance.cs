using System.Threading.Tasks;

namespace Sidekick.Common.Blazor.Views
{
    /// <summary>
    /// Interface to manage a view instance
    /// </summary>
    public interface IViewInstance
    {
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
        /// Closes the view and opens a new view
        /// </summary>
        Task CloseAndOpen(string url);

        /// <summary>
        /// The title of the view
        /// </summary>
        string Title { get; }
    }
}
