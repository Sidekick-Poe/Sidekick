using Microsoft.Extensions.Logging;
using Microsoft.JSInterop;
namespace Sidekick.Common.Ui.Utilities;

public class SafeJsObjectReference(
    IJSObjectReference reference,
    ILogger logger)
{
    public async Task InvokeVoidAsync(string identifier, params object?[]? args)
    {
        try
        {
            await reference.InvokeVoidAsync(identifier, args);
        }
        catch (Exception e) when (e is JSDisconnectedException or ObjectDisposedException)
        {
            // Expected if the underlying JS runtime/object was already disposed.
        }
        catch (Exception e) when (e.InnerException is JSDisconnectedException or ObjectDisposedException)
        {
            // Expected if the underlying JS runtime/object was already disposed.
        }
        catch (InvalidOperationException e) when (e.Message.Contains("JavaScript interop calls cannot be issued", StringComparison.OrdinalIgnoreCase))
        {
            // Defensive fallback for similar runtime-disposal cases.
        }
        catch (Exception e)
        {
            logger.LogDebug(e, "[SafeJsObjectReference] Failed to dispose javascript object.");
        }
    }

    public async ValueTask DisposeAsync()
    {
        try
        {
            await reference.DisposeAsync();
        }
        catch (Exception e) when (e is JSDisconnectedException or ObjectDisposedException)
        {
            // Expected when the Blazor circuit/WebView is already disconnected during disposal.
        }
        catch (Exception e) when (e.InnerException is JSDisconnectedException or ObjectDisposedException)
        {
            // Expected when the Blazor circuit/WebView is already disconnected during disposal.
        }
        catch (InvalidOperationException e) when (e.Message.Contains("JavaScript interop calls cannot be issued", StringComparison.OrdinalIgnoreCase))
        {
            // Defensive fallback for similar runtime-disposal cases.
        }
        catch (Exception e)
        {
            logger.LogDebug(e, "[SafeJsObjectReference] Failed to dispose javascript object.");
        }
    }
}
