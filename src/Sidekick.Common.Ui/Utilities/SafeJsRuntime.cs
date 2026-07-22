using Microsoft.Extensions.Logging;
using Microsoft.JSInterop;
namespace Sidekick.Common.Ui.Utilities;

public class SafeJsRuntime(
    IJSRuntime jsRuntime,
    ILogger<SafeJsRuntime> logger)
{
    public async Task InvokeVoidAsync(string identifier, params object?[]? args)
    {
        try
        {
            await jsRuntime.InvokeVoidAsync(identifier, args);
        }
        catch (JSDisconnectedException)
        {
            // The circuit was disconnected before the component finished initializing.
        }
        catch (ObjectDisposedException)
        {
            // The JS runtime or component was disposed before initialization completed.
        }
        catch (Exception e)
        {
            logger.LogError(e, "Failed to load javascript module.");
        }
    }

    public async Task<SafeJsObjectReference?> InvokeObjectAsync(string identifier, params object?[]? args)
    {
        try
        {
            var reference = await jsRuntime.InvokeAsync<IJSObjectReference>(identifier, args);
            return new SafeJsObjectReference(reference, logger);
        }
        catch (JSDisconnectedException)
        {
            // The circuit was disconnected before the component finished initializing.
        }
        catch (ObjectDisposedException)
        {
            // The JS runtime or component was disposed before initialization completed.
        }
        catch (Exception e)
        {
            logger.LogError(e, "Failed to load javascript module.");
        }

        return null;
    }
}
