using System.Runtime.InteropServices;
using Microsoft.Extensions.Logging;
using Sidekick.Common.Settings;

namespace Sidekick.Common.Platform.Clipboard
{
    /// <inheritdoc/>
    public class ClipboardProvider(
        ISettingsService settingsService,
        IKeyboardProvider keyboard,
        ILogger<ClipboardProvider> logger) : IClipboardProvider
    {
        /// <inheritdoc/>
        public async Task<string?> Copy()
        {
            var clipboardText = string.Empty;
            var retainClipboard = await settingsService.GetBool(SettingKeys.RetainClipboard);
            if (retainClipboard)
            {
                clipboardText = await GetText();
                logger.LogDebug("[Clipboard] Retained clipboard.");
            }

            await SetText(string.Empty);

            await keyboard.PressKey("Ctrl+C");
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
            string? data = null;
            Exception? exception = null;
            var staThread = new Thread(
                delegate()
                {
                    try
                    {
                        data = TextCopy.ClipboardService.GetText();
                    }
                    catch (Exception ex)
                    {
                        exception = ex;
                    }
                });
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                staThread.SetApartmentState(ApartmentState.STA);
            }

            staThread.Start();
            staThread.Join();

            if (exception != null)
            {
                logger.LogError(exception, "[Clipboard] Failed to get the text value from the clipboard.");
            }

            return Task.FromResult(data);
        }

        /// <inheritdoc/>
        public Task SetText(string? text)
        {
            text ??= string.Empty;
            Exception? exception = null;
            var staThread = new Thread(
                delegate()
                {
                    try
                    {
                        TextCopy.ClipboardService.SetText(text);
                    }
                    catch (Exception ex)
                    {
                        exception = ex;
                    }
                });
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                staThread.SetApartmentState(ApartmentState.STA);
            }

            staThread.Start();
            staThread.Join();

            if (exception != null)
            {
                logger.LogError(exception, "[Clipboard] Failed to set the text value to the clipboard.");
            }

            return Task.CompletedTask;
        }
    }
}
