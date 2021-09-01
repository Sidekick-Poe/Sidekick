using System.Threading.Tasks;
using Sidekick.Apis.Poe;
using Sidekick.Common.Platform;
using Sidekick.Domain.Keybinds;

namespace Sidekick.Electron.Keybinds
{
    public class FindItemKeybindHandler : IKeybindHandler
    {
        private readonly IKeyboardProvider keyboard;
        private readonly IClipboardProvider clipboardProvider;
        private readonly IProcessProvider processProvider;
        private readonly IItemParser itemParser;

        public FindItemKeybindHandler(
            IKeyboardProvider keyboard,
            IClipboardProvider clipboardProvider,
            IProcessProvider processProvider,
            IItemParser itemParser)
        {
            this.keyboard = keyboard;
            this.clipboardProvider = clipboardProvider;
            this.processProvider = processProvider;
            this.itemParser = itemParser;
        }

        public bool IsValid() => processProvider.IsPathOfExileInFocus;

        public async Task Execute()
        {
            var text = await clipboardProvider.Copy();
            var item = itemParser.ParseItem(text);

            if (item != null)
            {
                await clipboardProvider.SetText(item.Original.Name);
                keyboard.PressKey("Ctrl+F", "Ctrl+A", "Paste", "Enter");
            }
            else
            {
                // If we are not hovering over an item, we still want to put the focus in the search bar inside the game.
                keyboard.PressKey("Ctrl+F");
            }
        }
    }
}
