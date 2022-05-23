using System.Collections.Generic;
using System.Threading.Tasks;
using Sidekick.Common.Blazor.Views;
using Sidekick.Common.Extensions;
using Sidekick.Common.Platform;
using Sidekick.Common.Platform.Keybinds;
using Sidekick.Common.Settings;

namespace Sidekick.Modules.Trade.Keybinds
{
    public class PriceCheckItemKeybindHandler : IKeybindHandler
    {
        private readonly IViewLocator viewLocator;
        private readonly IClipboardProvider clipboardProvider;
        private readonly IProcessProvider processProvider;
        private readonly ISettings settings;

        public PriceCheckItemKeybindHandler(
            IViewLocator viewLocator,
            IClipboardProvider clipboardProvider,
            IProcessProvider processProvider,
            ISettings settings)
        {
            this.viewLocator = viewLocator;
            this.clipboardProvider = clipboardProvider;
            this.processProvider = processProvider;
            this.settings = settings;
        }

        public List<string> GetKeybinds() => new() { settings.Trade_Key_Check };

        public bool IsValid() => processProvider.IsPathOfExileInFocus;

        public async Task Execute(string _)
        {
            var itemText = await clipboardProvider.Copy();
            await viewLocator.Open($"/trade/{itemText.EncodeBase64Url()}");
        }
    }
}
