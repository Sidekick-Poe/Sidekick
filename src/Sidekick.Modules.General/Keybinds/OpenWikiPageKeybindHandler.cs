using Sidekick.Apis.Poe.Items;
using Sidekick.Apis.Poe.Languages;
using Sidekick.Apis.Poe.Trade;
using Sidekick.Apis.Poe.Trade.Parser;
using Sidekick.Common.Browser;
using Sidekick.Common.Exceptions;
using Sidekick.Common.Platform;
using Sidekick.Common.Platform.Input;
using Sidekick.Common.Settings;
using Sidekick.Modules.General.Settings;

namespace Sidekick.Modules.General.Keybinds;

public class OpenWikiPageKeybindHandler(
    IClipboardProvider clipboardProvider,
    ISettingsService settingsService,
    IProcessProvider processProvider,
    IItemParser itemParser,
    ICurrentGameLanguage currentGameLanguage,
    IBrowserProvider browserProvider,
    IKeyboardProvider keyboard) : KeybindHandler(settingsService, SettingKeys.KeyOpenWiki)
{
    private readonly ISettingsService settingsService = settingsService;

    protected override async Task<List<string?>> GetKeybinds() =>
    [
        await settingsService.GetString(SettingKeys.KeyOpenWiki)
    ];

    public override bool IsValid(string _) => processProvider.IsPathOfExileInFocus;

    public override async Task Execute(string keybind)
    {
        var text = await clipboardProvider.Copy();
        if (text == null)
        {
            await keyboard.PressKey(keybind);
            return;
        }

        var item = itemParser.ParseItem(text);

        var wikiPreferred = await settingsService.GetEnum<WikiSetting>(SettingKeys.PreferredWiki);
        if (wikiPreferred == WikiSetting.PoeWiki)
        {
            if (!currentGameLanguage.IsEnglish())
            {
                throw new UnavailableTranslationException();
            }

            OpenPoeWiki(item);
        }
        else if (wikiPreferred == WikiSetting.PoeDb)
        {
            OpenPoeDb(item);
        }
    }

    private const string PoeWikiBaseUri = "https://www.poewiki.net/";
    private const string Poe2WikiBaseUri = "https://www.poe2wiki.net/";
    private const string PoeWikiSubUrl = "w/index.php?search=";

    private void OpenPoeWiki(Item item)
    {
        var searchLink = item.ApiInformation.Name ?? item.ApiInformation.Type;
        var baseUrl = item.Game == GameType.PathOfExile1 ? PoeWikiBaseUri : Poe2WikiBaseUri;
        var wikiLink = PoeWikiSubUrl + searchLink?.Replace(" ", "+");
        var uri = new Uri(baseUrl + wikiLink);

        browserProvider.OpenUri(uri);
    }

    private const string PoeDbBaseUri = "https://poedb.tw/";
    private const string Poe2DbBaseUri = "https://poe2db.tw/";
    private const string PoeDbSubUrl = "search?q=";

    private void OpenPoeDb(Item item)
    {
        var searchLink = item.ApiInformation.Name ?? item.ApiInformation.Type;
        var baseUrl = item.Game == GameType.PathOfExile1 ? PoeDbBaseUri : Poe2DbBaseUri;
        var wikiLink = PoeDbSubUrl + searchLink?.Replace(" ", "+");
        var uri = new Uri(baseUrl + wikiLink);

        browserProvider.OpenUri(uri);
    }
}
