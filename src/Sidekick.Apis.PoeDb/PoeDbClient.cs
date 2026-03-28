using Sidekick.Apis.Poe.Items;
using Sidekick.Common.Browser;
using Sidekick.Data;
using Sidekick.Data.Languages;
namespace Sidekick.Apis.PoeDb;

public class PoeDbClient(
    ICurrentGameLanguage currentGameLanguage,
    IBrowserProvider browserProvider) : IPoeDbClient
{
    private const string PoeDbBaseUri = "https://poedb.tw/";
    private const string Poe2DbBaseUri = "https://poe2db.tw/";

    public void OpenWebsite(Item item)
    {
        var baseUrl = item.Game == GameType.PathOfExile1 ? PoeDbBaseUri : Poe2DbBaseUri;
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
        string? searchValue = null;
        if (!string.IsNullOrEmpty(item.Definition.UniqueItem?.Name)) searchValue = item.Definition.UniqueItem.Name;
        else if (!string.IsNullOrEmpty(item.Definition.TradeItem?.Name)) searchValue = item.Definition.TradeItem?.Name;
        else if (!string.IsNullOrEmpty(item.Definition.TradeItem?.Text)) searchValue = item.Definition.TradeItem?.Text;
        else if (!string.IsNullOrEmpty(item.Definition.TradeItem?.Type)) searchValue = item.Definition.TradeItem?.Type;
        else if (!string.IsNullOrEmpty(item.Definition.BaseItem?.Name)) searchValue = item.Definition.BaseItem.Name;
        return searchValue;
    }
}
