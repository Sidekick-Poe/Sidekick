using Sidekick.Apis.Poe.Trade.Parser;
using Sidekick.Apis.PoeDb;
using Sidekick.Apis.PoeWiki;
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
    IKeyboardProvider keyboard,
    IPoeDbClient poeDbClient,
    IPoeWikiClient poeWikiClient) : KeybindHandler(settingsService, SettingKeys.KeyOpenWiki)
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
        switch (wikiPreferred)
        {
            case WikiSetting.PoeWiki:
                poeWikiClient.OpenWebsite(item);
                break;
            case WikiSetting.PoeDb:
                poeDbClient.OpenWebsite(item);
                break;
        }
    }
}
