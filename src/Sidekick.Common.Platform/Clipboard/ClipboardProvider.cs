using System.Threading.Tasks;
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
        public async Task<string> Copy()
        {
            var clipboardText = string.Empty;

            if (settings.RetainClipboard)
            {
                clipboardText = await TextCopy.ClipboardService.GetTextAsync();
                if (clipboardText == null)
                {
                    clipboardText = string.Empty;
                }
            }

            await SetText(string.Empty);

            keyboard.PressKey("Copy");

            await Task.Delay(100);

            // Retrieve clipboard.
            var result = await TextCopy.ClipboardService.GetTextAsync();

            if (settings.RetainClipboard)
            {
                await Task.Delay(100);
                await TextCopy.ClipboardService.SetTextAsync(clipboardText);
            }

            return result;
        }

        /// <inheritdoc/>
        public async Task<string> GetText()
        {
            return await TextCopy.ClipboardService.GetTextAsync();
        }

        /// <inheritdoc/>
        public async Task SetText(string text)
        {
            if (text == null)
            {
                text = string.Empty;
            }
            await TextCopy.ClipboardService.SetTextAsync(text);
        }
    }
}
