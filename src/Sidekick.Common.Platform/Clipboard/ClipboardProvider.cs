using System.Runtime.InteropServices;
using Microsoft.Extensions.Logging;
using Sidekick.Common.Settings;

namespace Sidekick.Common.Platform.Clipboard;

/// <inheritdoc/>
public class ClipboardProvider
(
    ISettingsService settingsService,
    IKeyboardProvider keyboard,
    ILogger<ClipboardProvider> logger
) : IClipboardProvider
{
    /// <inheritdoc/>
    public async Task<string?> Copy(bool? withAlt = false)
    {
        var clipboardText = string.Empty;
        var retainClipboard = await settingsService.GetBool(SettingKeys.RetainClipboard);
        if (retainClipboard)
        {
            clipboardText = await GetText();
            logger.LogDebug("[Clipboard] Retained clipboard.");
        }

        await SetText(string.Empty);

        // Alt information is not used for item parsing.
        if (withAlt == true)
        {
            keyboard.ReleaseAltModifier();
            await keyboard.PressKey("Ctrl+Alt+C");
        }
        else
        {
            await keyboard.PressKey("Ctrl+C");
        }

        logger.LogDebug("[Clipboard] Sent keystrokes Ctrl+C");

        await Task.Delay(100);

        // Retrieve clipboard.
        var result = await GetText();
        logger.LogDebug("[Clipboard] Fetched clipboard value.");

        if (retainClipboard)
        {
            await Task.Delay(100);
            await SetText(clipboardText);
            logger.LogDebug("[Clipboard] Reset clipboard to retained value.");
        }

        if (string.IsNullOrEmpty(result))
        {
            return null;
        }

        return result;
    }

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

        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
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

        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            staThread.SetApartmentState(ApartmentState.STA);
        }

        staThread.Start();

        return tcs.Task;
    }
}
