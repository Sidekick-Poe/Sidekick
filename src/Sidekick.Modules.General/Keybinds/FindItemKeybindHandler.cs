using System.Collections.Generic;
using System.Threading.Tasks;
using Sidekick.Apis.Poe;
using Sidekick.Common;
using Sidekick.Common.Platform;
using Sidekick.Common.Settings;

namespace Sidekick.Modules.General.Keybinds
{
    public class FindItemKeybindHandler : IKeybindHandler
    {
        private readonly IKeyboardProvider keyboard;
        private readonly IClipboardProvider clipboardProvider;
        private readonly IProcessProvider processProvider;
        private readonly IItemParser itemParser;
        private readonly ISettings settings;

        public FindItemKeybindHandler(
            IKeyboardProvider keyboard,
            IClipboardProvider clipboardProvider,
            IProcessProvider processProvider,
            IItemParser itemParser,
            ISettings settings)
        {
            this.keyboard = keyboard;
            this.clipboardProvider = clipboardProvider;
            this.processProvider = processProvider;
            this.itemParser = itemParser;
            this.settings = settings;
        }

        public List<string> GetKeybinds() => new() { settings.Key_FindItems };

        public bool IsValid() => processProvider.IsPathOfExileInFocus;

        public async Task Execute(string _)
        {
            var text = await clipboardProvider.Copy();
            var item = itemParser.ParseItem(text);

            if (item != null)
            {
                await clipboardProvider.SetText(item.Original.Name);
                await keyboard.PressKey("Ctrl+F", "Ctrl+A", "Ctrl+V", "Enter");
            }
            else
            {
                // If we are not hovering over an item, we still want to put the focus in the search
                // bar inside the game.
                await keyboard.PressKey("Ctrl+F");
            }
        }
    }
}
