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

            return result;
        }

        /// <inheritdoc/>
        public async Task<string?> GetText()
        {
            return await TextCopy.ClipboardService.GetTextAsync();
        }

        /// <inheritdoc/>
        public async Task SetText(string? text)
        {
            if (text == null)
            {
                text = string.Empty;
            }
            await TextCopy.ClipboardService.SetTextAsync(text);
        }
    }
}
