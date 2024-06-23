using Sidekick.Apis.Poe;
using Sidekick.Common;
using Sidekick.Common.Browser;
using Sidekick.Common.Exceptions;
using Sidekick.Common.Game.Items;
using Sidekick.Common.Game.Languages;
using Sidekick.Common.Keybinds;
using Sidekick.Common.Platform;

namespace Sidekick.Modules.General.Keybinds
{
    public class OpenWikiPageKeybindHandler(
        IClipboardProvider clipboardProvider,
        ISettingsService settingsService,
        IProcessProvider processProvider,
        IItemParser itemParser,
        IGameLanguageProvider gameLanguageProvider,
        IBrowserProvider browserProvider,
        IKeyboardProvider keyboard) : IKeybindHandler
    {
        public List<string?> GetKeybinds() =>
        [
            settingsService.GetSettings()
                           .Wiki_Key_Open,
        ];

        public bool IsValid(string _) => processProvider.IsPathOfExileInFocus;

        public async Task Execute(string keybind)
        {
            var text = await clipboardProvider.Copy();
            if (text == null)
            {
                await keyboard.PressKey(keybind);
                return;
            }

            var item = await itemParser.ParseItemAsync(text);

            var settings = settingsService.GetSettings();
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

        private const string PoeWikiBaseUri = "https://www.poewiki.net/";
        private const string PoeWikiSubUrl = "w/index.php?search=";

        private void OpenPoeWiki(Item item)
        {
            var searchLink = item.Metadata.Name ?? item.Metadata.Type;
            var wikiLink = PoeWikiSubUrl + searchLink?.Replace(" ", "+");
            var uri = new Uri(PoeWikiBaseUri + wikiLink);

            browserProvider.OpenUri(uri);
        }

        private const string PoeDbBaseUri = "https://poedb.tw/";
        private const string PoeDbSubUrl = "search?q=";

        private void OpenPoeDb(Item item)
        {
            var searchLink = item.Metadata.Name ?? item.Metadata.Type;
            var wikiLink = PoeDbSubUrl + searchLink?.Replace(" ", "+");
            var uri = new Uri(PoeDbBaseUri + wikiLink);

            browserProvider.OpenUri(uri);
        }
    }
}
