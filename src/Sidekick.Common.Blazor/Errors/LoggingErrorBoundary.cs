using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.Logging;
using Microsoft.JSInterop;

namespace Sidekick.Common.Blazor.Errors
{
    /// <summary>
    /// Captures errors thrown from its child content.
    /// </summary>
    public class LoggingErrorBoundary : ErrorBoundary
    {
        [Inject]
        private ILogger<LoggingErrorBoundary> Logger { get; set; } = null!;

        [Inject]
        private IJSRuntime JsRuntime { get; set; } = null!;

        public new Exception? CurrentException { get; private set; }

        protected override async Task OnErrorAsync(Exception exception)
        {
            if (exception is AggregateException aggregateException)
            {
                CurrentException = aggregateException.InnerException;
            }
            else
            {
                CurrentException = exception;
            }

            await JsRuntime.InvokeVoidAsync("console.error", CurrentException.ToString());
            Logger.LogError(CurrentException, "An error occured while executing a component.");

            await base.OnErrorAsync(exception);
        }
    }
}
