using Microsoft.AspNetCore.Components.Web;

namespace Sidekick.Common.Blazor.Errors
{
    /// <summary>
    /// Captures errors thrown from its child content.
    /// </summary>
    public class SidekickErrorBoundary : ErrorBoundary
    {
        /// <summary>
        /// Gets the current exception, or null if there is no exception.
        /// </summary>
        public new Exception? CurrentException => base.CurrentException;
    }
}
