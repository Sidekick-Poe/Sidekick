using Sidekick.Apis.Poe.Extensions;
using Sidekick.Apis.Poe.Languages;
using Sidekick.Common.Settings;
using Sidekick.Data.Trade;
using Sidekick.Data.Trade.Models.Raw;

namespace Sidekick.Apis.Poe.Trade.ApiStatic;

public class ApiStaticDataProvider
(
    TradeDataProvider tradeDataProvider,
    ICurrentGameLanguage currentGameLanguage,
    ISettingsService settingsService
) : IApiStaticDataProvider
{
    private Dictionary<string, RawTradeStaticItem> TextDictionary { get; } = new();
    private Dictionary<string, RawTradeStaticItem> IdDictionary { get; } = new();

    /// <inheritdoc/>
    public int Priority => 100;

    /// <inheritdoc/>
    public async Task Initialize()
    {
        var game = await settingsService.GetGame();
        var result = await tradeDataProvider.GetStaticItems(game, currentGameLanguage.Language.Code);

        TextDictionary.Clear();
        IdDictionary.Clear();

        foreach (var category in result)
        {
            foreach (var entry in category.Entries)
            {
                if (entry.Id == null! || entry.Text == null || entry.Id == "sep") continue;

                entry.Image = $"https://web.poecdn.com{entry.Image}";
                TextDictionary.TryAdd(entry.Text, entry);
                IdDictionary.TryAdd(entry.Id, entry);
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

        return IdDictionary.GetValueOrDefault(id);
    }

    public RawTradeStaticItem? Get(string? name, string? type)
    {
        var data = !string.IsNullOrEmpty(name) ? TextDictionary.GetValueOrDefault(name) : null;
        data ??= !string.IsNullOrEmpty(type) ? TextDictionary.GetValueOrDefault(type) : null;
        if (data == null) return null;

        return GetById(data.Id);
    }

}
