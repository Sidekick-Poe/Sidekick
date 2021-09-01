using System;
using System.Threading.Tasks;
using Sidekick.Apis.Poe;
using Sidekick.Common.Blazor.Views;
using Sidekick.Common.Browser;
using Sidekick.Common.Game.Items;
using Sidekick.Common.Game.Languages;
using Sidekick.Common.Platform;
using Sidekick.Common.Settings;
using Sidekick.Domain.Keybinds;

namespace Sidekick.Electron.Keybinds
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

        public OpenWikiPageKeybindHandler(
            IClipboardProvider clipboardProvider,
            IViewLocator viewLocator,
            ISettings settings,
            IProcessProvider processProvider,
            IItemParser itemParser,
            IGameLanguageProvider gameLanguageProvider,
            IBrowserProvider browserProvider)
        {
            this.clipboardProvider = clipboardProvider;
            this.viewLocator = viewLocator;
            this.settings = settings;
            this.processProvider = processProvider;
            this.itemParser = itemParser;
            this.gameLanguageProvider = gameLanguageProvider;
            this.browserProvider = browserProvider;
        }

        public bool IsValid() => processProvider.IsPathOfExileInFocus;

        public async Task Execute()
        {
            var text = await clipboardProvider.Copy();
            var item = itemParser.ParseItem(text);

            if (item == null)
            {
                // If the item can't be parsed, show an error
                await viewLocator.Open("/error/unparsable");
                return;
            }

            if (!gameLanguageProvider.IsEnglish())
            {
                // Only available for english language
                await viewLocator.Open("/error/unavailable");
                return;
            }

            if (string.IsNullOrEmpty(item.Metadata.Name))
            {
                // Most items will open the basetype wiki link.
                // Does not work for unique items that are not identified.
                await viewLocator.Open("/error/invalid");
                return;
            }

            if (settings.Wiki_Preferred == WikiSetting.PoeDb)
            {
                OpenPoeDb(item);
            }
            else
            {
                OpenPoeWiki(item);
            }
        }

        #region PoeDb
        private const string PoeDb_BaseUri = "https://poedb.tw/";
        private const string PoeDb_SubUrlUnique = "unique.php?n=";
        private const string PoeDb_SubUrlGem = "gem.php?n=";
        private const string PoeDb_SubUrlItem = "item.php?n=";
        private void OpenPoeDb(Item item)
        {
            var subUrl = item.Metadata.Rarity switch
            {
                Rarity.Unique => PoeDb_SubUrlUnique,
                Rarity.Gem => PoeDb_SubUrlGem,
                _ => PoeDb_SubUrlItem
            };

            var searchLink = item.Metadata.Name ?? item.Metadata.Type;
            var wikiLink = subUrl + searchLink.Replace(" ", "+");

            browserProvider.OpenUri(new Uri(PoeDb_BaseUri + wikiLink));
        }
        #endregion

        #region PoeWiki
        private void OpenPoeWiki(Item item)
        {
            // determine search link, so wiki can be opened for any item
            var searchLink = item.Metadata.Name ?? item.Metadata.Type;
            // replace space encodes with '_' to match the link layout of the poe wiki and then url encode it
            var itemLink = System.Net.WebUtility.UrlEncode(searchLink.Replace(" ", "_"));

            browserProvider.OpenUri(new Uri($"https://pathofexile.gamepedia.com/{itemLink}"));
        }
        #endregion
    }
}
