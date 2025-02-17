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

        await JsRuntime.InvokeVoidAsync("console.error", CurrentException?.ToString());
        Logger.LogError(CurrentException, "[ErrorBoundary] An error occured.");

        await base.OnErrorAsync(exception);

        CurrentView.SetSize(minHeight: 360);
    }
}
