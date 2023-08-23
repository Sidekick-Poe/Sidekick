using Sidekick.Common.Settings;

namespace Sidekick.Common.Platform.Clipboard
{
    /// <inheritdoc/>
    public class ClipboardProvider : IClipboardProvider
    {
        private readonly ISettings settings;
        private readonly IKeyboardProvider keyboard;

        public ClipboardProvider(
            ISettings settings,
            IKeyboardProvider keyboard)
        {
            this.settings = settings;
            this.keyboard = keyboard;
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
                clipboardText = await TextCopy.ClipboardService.GetTextAsync();
            }

            await SetText(string.Empty);

            keyboard.PressKey(keyStroke);

            await Task.Delay(100);

            // Retrieve clipboard.
            var result = await TextCopy.ClipboardService.GetTextAsync();

            if (settings.RetainClipboard)
            {
                await Task.Delay(100);
                await SetText(clipboardText);
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
