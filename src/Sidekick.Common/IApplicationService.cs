using System.Threading.Tasks;

namespace Sidekick.Common.Platform
{
    /// <summary>
    /// Interface containing platform specific methods.
    /// </summary>
    public interface IApplicationService
    {
        /// <summary>
        /// Shutdown the application.
        /// </summary>
        void Shutdown();

        /// <summary>
        /// Open a notification message with Yes/No buttons
        /// </summary>
        /// <param name="message">The message to show in the notification</param>
        /// <returns>True if the user confirmed the action.</returns>
        Task<bool> OpenConfirmationModal(string message);
    }
}
