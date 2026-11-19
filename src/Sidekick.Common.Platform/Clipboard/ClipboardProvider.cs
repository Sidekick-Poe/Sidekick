using System.Diagnostics;
using System.Runtime.InteropServices;
using Microsoft.Extensions.Logging;
using Sidekick.Common.Settings;

namespace Sidekick.Common.Platform.Clipboard;

/// <inheritdoc/>
public class ClipboardProvider
(
    ISettingsService settingsService,
    IInputProvider input,
    ILogger<ClipboardProvider> logger
) : IClipboardProvider
{
    /// <summary>
    /// The text put in the clipboard before copying, so a failed copy can be detected. An empty text would not work,
    /// since setting it on Linux keeps the previous text. Users can paste it by accident, so it says where it comes from.
    /// </summary>
    private const string CopyPlaceholderText = "Sidekick tried to copy an item from Path of Exile, but no item was copied.";

    private static readonly TimeSpan CopyPollInterval = TimeSpan.FromMilliseconds(50);

    private static readonly TimeSpan WineClipboardSyncDelay = TimeSpan.FromMilliseconds(50);

    /// <summary>
    /// How long to wait for the copied text before deciding nothing was copied (cursor not over an item).
    /// </summary>
    private static readonly TimeSpan CopyTimeout = TimeSpan.FromMilliseconds(500);

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

        await SetText(CopyPlaceholderText);

        // Wine (and Proton, Valve's Wine that runs the game through Steam) imports every clipboard change into its own
        // clipboard, in the background. Without a pause, importing the placeholder can land after the game's copy and
        // replace it, and quick clipboard changes can make Wine stop answering clipboard requests until it is restarted.
        if (OperatingSystem.IsLinux())
        {
            await Task.Delay(WineClipboardSyncDelay);
        }

        if (withAlt == true)
        {
            await input.PressKey("Ctrl+Alt+C");
        }
        else
        {
            input.ReleaseAltModifier();
            await input.PressKey("Ctrl+C");
        }

        logger.LogDebug("[Clipboard] Sent keystrokes Ctrl+C");

        var result = await WaitForCopiedText();

        if (retainClipboard)
        {
            await SetText(clipboardText);
            logger.LogDebug("[Clipboard] Reset clipboard to retained value.");
        }

        return result;
    }

    /// <summary>
    /// Checks the clipboard until the copied text replaces the placeholder, so the price check starts as soon as the
    /// copy arrives instead of after a fixed wait that could also be too short (the game may be busy).
    /// </summary>
    private async Task<string?> WaitForCopiedText()
    {
        var stopwatch = Stopwatch.StartNew();
        while (true)
        {
            await Task.Delay(CopyPollInterval);

            var text = await GetText();
            if (!string.IsNullOrEmpty(text) && text != CopyPlaceholderText)
            {
                logger.LogDebug("[Clipboard] Fetched the copied text after {Elapsed} ms.", stopwatch.ElapsedMilliseconds);
                return text;
            }

            if (stopwatch.Elapsed >= CopyTimeout)
            {
                logger.LogDebug("[Clipboard] Nothing was copied after {Elapsed} ms.", stopwatch.ElapsedMilliseconds);
                return null;
            }
        }
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
