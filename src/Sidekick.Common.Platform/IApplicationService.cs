using System;
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
        /// <param name="title">The title of the notification</param>
        /// <param name="onYes">The action to execute when the Yes button is clicked</param>
        /// <param name="onNo">The action to execute when the No button is clicked</param>
        Task OpenConfirmationNotification(string message, string title = null, Func<Task> onYes = null, Func<Task> onNo = null);
    }
}
