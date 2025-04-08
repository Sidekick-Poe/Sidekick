using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.Logging;
using Microsoft.JSInterop;
using Sidekick.Common.Ui.Views;

namespace Sidekick.Common.Ui.Errors;

/// <summary>
/// Captures errors thrown from its child content.
/// </summary>
public class LoggingErrorBoundary : ErrorBoundary
{
    [Inject]
    private ILogger<LoggingErrorBoundary> Logger { get; set; } = null!;

    [Inject]
    private IJSRuntime JsRuntime { get; set; } = null!;

    [Inject]
    private ICurrentView CurrentView { get; set; } = null!;

    public Exception? CapturedException => CurrentException;

    protected override async Task OnErrorAsync(Exception exception)
    {
        if (exception is AggregateException aggregateException && aggregateException.InnerException != null)
        {
            exception = aggregateException.InnerException;
        }

        await JsRuntime.InvokeVoidAsync("console.error", exception.ToString());
        Logger.LogError(exception, "[ErrorBoundary] An error occurred.");

        await base.OnErrorAsync(exception);

        CurrentView.Initialize(new ViewOptions());
    }
}
