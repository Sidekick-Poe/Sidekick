using Microsoft.Extensions.Logging;

namespace Sidekick.Common.Platform.Clipboard;

/// <summary>
/// The clipboard on Windows and in X11 sessions, through the TextCopy library (xsel on Linux).
/// </summary>
public class TextCopyClipboard(ILogger<TextCopyClipboard> logger) : ISystemClipboard
{
    /// <inheritdoc/>
    public Task<string?> GetText()
    {
        var tcs = new TaskCompletionSource<string?>();
        var staThread = new Thread(() =>
        {
            try
            {
                tcs.SetResult(TextCopy.ClipboardService.GetText());
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "[Clipboard] Failed to get the text value from the clipboard.");
                tcs.SetException(ex);
            }
        });

        if (OperatingSystem.IsWindows())
        {
            staThread.SetApartmentState(ApartmentState.STA);
        }

        staThread.Start();

        return tcs.Task;
    }

    /// <inheritdoc/>
    public Task SetText(string? text)
    {
        var tcs = new TaskCompletionSource();
        var staThread = new Thread(() =>
        {
            try
            {
                TextCopy.ClipboardService.SetText(text ?? string.Empty);
                tcs.SetResult();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "[Clipboard] Failed to set the text value to the clipboard.");
                tcs.SetException(ex);
            }
        });

        if (OperatingSystem.IsWindows())
        {
            staThread.SetApartmentState(ApartmentState.STA);
        }

        staThread.Start();

        return tcs.Task;
    }
}
