using System.Runtime.InteropServices;
using Microsoft.Extensions.Logging;
using Sidekick.Common.Settings;

namespace Sidekick.Common.Platform.Clipboard
{
    /// <inheritdoc/>
    public class ClipboardProvider : IClipboardProvider
    {
        private readonly ISettings settings;
        private readonly IKeyboardProvider keyboard;
        private readonly ILogger<ClipboardProvider> logger;

        public ClipboardProvider(
            ISettings settings,
            IKeyboardProvider keyboard,
            ILogger<ClipboardProvider> logger)
        {
            this.settings = settings;
            this.keyboard = keyboard;
            this.logger = logger;
        }

        /// <inheritdoc/>
        public Task<string?> Copy()
        {
            return ExecuteCopy("Ctrl+C");
        }

        /// <inheritdoc/>
        public Task<string?> CopyAdvanced()
        {
            return ExecuteCopy("Ctrl+Alt+C");
        }

        /// <inheritdoc/>
        private async Task<string?> ExecuteCopy(string keyStroke)
        {
            var clipboardText = string.Empty;
            if (settings.RetainClipboard)
            {
                clipboardText = await GetText();
                logger.LogDebug("[Clipboard] Retained clipboard.");
            }

            await SetText(string.Empty);

            await keyboard.PressKey(keyStroke);
            logger.LogDebug($"[Clipboard] Sent keystrokes {keyStroke}");

            await Task.Delay(100);

            // Retrieve clipboard.
            var result = await GetText();
            logger.LogDebug("[Clipboard] Fetched clipboard value.");

            if (settings.RetainClipboard)
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
                delegate ()
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
            if (text == null)
            {
                text = string.Empty;
            }

            Exception? exception = null;
            var staThread = new Thread(
                delegate ()
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
