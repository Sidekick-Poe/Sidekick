using Sidekick.Common.Browser;
using Sidekick.Common.Settings.Languages;
using Sidekick.Game;
using Sidekick.Game.Parser.Items;
namespace Sidekick.Apis.PoeDb;

public class PoeDbClient(
    ICurrentGameLanguage currentGameLanguage,
    IBrowserProvider browserProvider) : IPoeDbClient
{
    private const string PoeDbBaseUri = "https://poedb.tw/";
    private const string Poe2DbBaseUri = "https://poe2db.tw/";

    public void OpenWebsite(Item item)
    {
        var baseUrl = item.Game == GameType.Poe1 ? PoeDbBaseUri : Poe2DbBaseUri;
        var languageCodeSuffix = currentGameLanguage.Language.Code switch
        {
            "en" => "us/",
            "es" => "sp/",
            "zh" => "tw/",
            "ko" => "kr/",
            "ja" => "jp/",
            _ => currentGameLanguage.Language.Code + "/"
        };

        var searchValue = GetSearchValue(item)?.Replace(" ", "_");
        var uri = new Uri(baseUrl + languageCodeSuffix + searchValue);

        browserProvider.OpenUri(uri);
    }

    private string? GetSearchValue(Item item)
    {
        if (!string.IsNullOrEmpty(item.InvariantTradeItem?.Name)) return item.InvariantTradeItem?.Name;
        if (!string.IsNullOrEmpty(item.InvariantTradeItem?.Text)) return item.InvariantTradeItem?.Text;
        if (!string.IsNullOrEmpty(item.InvariantTradeItem?.Type)) return item.InvariantTradeItem?.Type;
        return null;
    }
}
