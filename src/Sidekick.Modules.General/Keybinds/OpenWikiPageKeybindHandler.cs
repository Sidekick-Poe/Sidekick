using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Sidekick.Apis.Poe;
using Sidekick.Common;
using Sidekick.Common.Blazor.Views;
using Sidekick.Common.Browser;
using Sidekick.Common.Exceptions;
using Sidekick.Common.Game.Items;
using Sidekick.Common.Game.Languages;
using Sidekick.Common.Platform;
using Sidekick.Common.Settings;

namespace Sidekick.Modules.General.Keybinds
{
    public class OpenWikiPageKeybindHandler : IKeybindHandler
    {
        private readonly IClipboardProvider clipboardProvider;
        private readonly IViewLocator viewLocator;
        private readonly ISettings settings;
        private readonly IProcessProvider processProvider;
        private readonly IItemParser itemParser;
        private readonly IGameLanguageProvider gameLanguageProvider;
        private readonly IBrowserProvider browserProvider;
        private readonly IKeyboardProvider keyboard;

        public OpenWikiPageKeybindHandler(
            IClipboardProvider clipboardProvider,
            IViewLocator viewLocator,
            ISettings settings,
            IProcessProvider processProvider,
            IItemParser itemParser,
            IGameLanguageProvider gameLanguageProvider,
            IBrowserProvider browserProvider,
            IKeyboardProvider keyboard)
        {
            this.clipboardProvider = clipboardProvider;
            this.viewLocator = viewLocator;
            this.settings = settings;
            this.processProvider = processProvider;
            this.itemParser = itemParser;
            this.gameLanguageProvider = gameLanguageProvider;
            this.browserProvider = browserProvider;
            this.keyboard = keyboard;
        }

        public List<string> GetKeybinds() => new() { settings.Wiki_Key_Open };

        public bool IsValid(string _) => processProvider.IsPathOfExileInFocus;

        public async Task Execute(string keybind)
        {
            var text = await clipboardProvider.Copy();
            if (text == null)
            {
                await keyboard.PressKey(keybind);
                return;
            }

            var item = itemParser.ParseItem(text);

            if (item == null)
            {
                throw new UnparsableException();
            }

            if (settings.Wiki_Preferred == WikiSetting.PoeWiki)
            {
                if (!gameLanguageProvider.IsEnglish())
                {
                    throw new UnavailableTranslationException();
                }

                OpenPoeWiki(item);
            }
            else if (settings.Wiki_Preferred == WikiSetting.PoeDb)
            {
                OpenPoeDb(item);
            }
        }

        private const string PoeWiki_BaseUri = "https://www.poewiki.net/";
        private const string PoeWiki_SubUrl = "w/index.php?search=";

        private void OpenPoeWiki(Item item)
        {
            var searchLink = item.Metadata.Name ?? item.Metadata.Type;
            var wikiLink = PoeWiki_SubUrl + searchLink.Replace(" ", "+");
            var uri = new Uri(PoeWiki_BaseUri + wikiLink);

            browserProvider.OpenUri(uri);
        }

        private const string PoeDb_BaseUri = "https://poedb.tw/";
        private const string PoeDb_SubUrl = "search?q=";

        private void OpenPoeDb(Item item)
        {
            var searchLink = item.Metadata.Name ?? item.Metadata.Type;
            var wikiLink = PoeDb_SubUrl + searchLink.Replace(" ", "+");
            var uri = new Uri(PoeDb_BaseUri + wikiLink);

            browserProvider.OpenUri(uri);
        }
    }
}
