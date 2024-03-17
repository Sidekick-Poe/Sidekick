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
        private IJSRuntime JSRuntime { get; set; } = null!;

        public new Exception? CurrentException { get; private set; }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (CurrentException != null)
            {
                await JSRuntime.InvokeVoidAsync("console.error", CurrentException.ToString());
                Logger.LogError(CurrentException, "An error occured while executing a component.");
            }

            await base.OnAfterRenderAsync(firstRender);
        }
    }
}
