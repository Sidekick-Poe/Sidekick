using Sidekick.Apis.Poe.Extensions;
using Sidekick.Apis.Poe.Items;
using Sidekick.Apis.Poe.Languages;
using Sidekick.Common.Settings;
using Sidekick.Data.Trade;
using Sidekick.Data.Trade.Models.Raw;
namespace Sidekick.Apis.Poe.Trade.ApiStatic;

public class ApiStaticDataProvider
(
    TradeDataProvider tradeDataProvider,
    IGameLanguageProvider gameLanguageProvider,
    ISettingsService settingsService
) : IApiStaticDataProvider
{
    private Dictionary<string, RawTradeStaticItem> TextDictionary { get; } = new();
    private Dictionary<string, RawTradeStaticItem> InvariantDictionary { get; } = new();

    /// <inheritdoc/>
    public int Priority => 100;

    /// <inheritdoc/>
    public async Task Initialize()
    {
        var game = await settingsService.GetGame();
        await InitializeText(game);
        await InitializeInvariant(game);
    }

    private async Task InitializeText(GameType game)
    {
        var result = await tradeDataProvider.GetStaticItems(game, gameLanguageProvider.Language.Code);

        TextDictionary.Clear();
        FillDictionary(TextDictionary, result, x => x.Text);
    }

    private async Task InitializeInvariant(GameType game)
    {
        var result = await tradeDataProvider.GetStaticItems(game, gameLanguageProvider.InvariantLanguage.Code);

        InvariantDictionary.Clear();
        FillDictionary(InvariantDictionary, result, x => x.Id);
    }

    private void FillDictionary(Dictionary<string, RawTradeStaticItem> dictionary, List<RawTradeStaticItemCategory> result, Func<RawTradeStaticItem, string?> keyFunc)
    {
        foreach (var category in result)
        {
            foreach (var entry in category.Entries)
            {
                var key = keyFunc(entry);
                if (key == null || entry.Id == null! || entry.Text == null || entry.Id == "sep") continue;

                entry.Image = $"https://web.poecdn.com{entry.Image}";
                dictionary.TryAdd(key, entry);
            }
        }
    }

    public RawTradeStaticItem? GetById(string? id)
    {
        if (string.IsNullOrEmpty(id)) return null;

        id = id switch
        {
            "exalt" => "exalted",
            _ => id,
        };
        if (string.IsNullOrEmpty(id)) return null;

        return InvariantDictionary.GetValueOrDefault(id);
    }

    public RawTradeStaticItem? Get(string? name, string? type)
    {
        var data = !string.IsNullOrEmpty(name) ? TextDictionary.GetValueOrDefault(name) : null;
        data ??= !string.IsNullOrEmpty(type) ? TextDictionary.GetValueOrDefault(type) : null;
        if (data == null) return null;

        return GetById(data.Id);
    }

}
