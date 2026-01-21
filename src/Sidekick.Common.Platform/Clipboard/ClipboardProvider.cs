using System.Diagnostics;
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
    public async Task<string?> Copy(bool? withAlt = false, bool? assumeCtrlHeld = null)
    {
        var clipboardText = string.Empty;
        var retainClipboard = await settingsService.GetBool(SettingKeys.RetainClipboard);
        if (retainClipboard)
        {
            clipboardText = await GetText();
            logger.LogDebug("[Clipboard] Retained clipboard.");
        }

        await SetText(string.Empty);

        if (withAlt == true)
        {
            await keyboard.PressKey("Ctrl+Alt+C");
        }
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux) && assumeCtrlHeld == true)
        {
            // Ctrl is already held from the hotkey; avoid toggling it during the copy.
            keyboard.ReleaseAltModifier();
            await keyboard.PressKey("C");
        }
        else
        {
            keyboard.ReleaseAltModifier();
            await keyboard.PressKey("Ctrl+C");
        }

        logger.LogDebug("[Clipboard] Sent keystrokes Ctrl+C");

        var result = await ReadClipboardCopyResult();
        logger.LogDebug("[Clipboard] Fetched clipboard value.");

        if (retainClipboard)
        {
            await Task.Delay(100);
            await SetText(clipboardText);
            logger.LogDebug("[Clipboard] Reset clipboard to retained value.");
        }

        return string.IsNullOrEmpty(result) ? null : result;
    }

    private async Task<string?> ReadClipboardCopyResult()
    {
        if (!RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
        {
            await Task.Delay(100);
            return await GetText();
        }

        // On Linux, wait longer for the game to populate the clipboard.
        var timeout = TimeSpan.FromMilliseconds(750);
        var pollInterval = TimeSpan.FromMilliseconds(50);
        var stopwatch = Stopwatch.StartNew();
        string? result = null;

        while (stopwatch.Elapsed < timeout)
        {
            await Task.Delay(pollInterval);
            result = await GetText();
            if (!string.IsNullOrEmpty(result))
            {
                return result;
            }
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
