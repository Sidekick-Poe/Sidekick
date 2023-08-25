using System.Collections.Generic;
using System.Threading.Tasks;
using Sidekick.Common;
using Sidekick.Common.Blazor.Views;
using Sidekick.Common.Extensions;
using Sidekick.Common.Platform;
using Sidekick.Common.Settings;

namespace Sidekick.Modules.Maps.Keybinds
{
    public class OpenMapInfoKeybindHandler : IKeybindHandler
    {
        private readonly IViewLocator viewLocator;
        private readonly IClipboardProvider clipboardProvider;
        private readonly IProcessProvider processProvider;
        private readonly ISettings settings;

        public OpenMapInfoKeybindHandler(
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

        public List<string> GetKeybinds() => new() { settings.Map_Key_Check };

        public bool IsValid() => processProvider.IsPathOfExileInFocus;

        public async Task Execute(string _)
        {
            var itemText = await clipboardProvider.Copy();
            await viewLocator.Open($"/map/{itemText.EncodeBase64Url()}");
        }
    }
}
