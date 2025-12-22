using Sidekick.Apis.Poe.Extensions;
using Sidekick.Apis.Poe.Items;
using Sidekick.Apis.Poe.Languages;
using Sidekick.Apis.Poe.Trade.ApiStatic.Models;
using Sidekick.Apis.Poe.Trade.Clients;
using Sidekick.Apis.Poe.Trade.Clients.Models;
using Sidekick.Common.Cache;
using Sidekick.Common.Enums;
using Sidekick.Common.Exceptions;
using Sidekick.Common.Settings;
namespace Sidekick.Apis.Poe.Trade.ApiStatic;

public class ApiStaticDataProvider
(
    ICacheProvider cacheProvider,
    ITradeApiClient tradeApiClient,
    IGameLanguageProvider gameLanguageProvider,
    ISettingsService settingsService
) : IApiStaticDataProvider
{
    private Dictionary<string, StaticItem> TextDictionary { get; } = new();
    private Dictionary<string, StaticItem> InvariantDictionary { get; } = new();

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
        var cacheKey = $"{game.GetValueAttribute()}_StaticData";
        var result = await cacheProvider.GetOrSet(cacheKey, () => tradeApiClient.FetchData<StaticItemCategory>(game, gameLanguageProvider.Language, "static"), (cache) => cache.Result.Any());
        if (result == null) throw new SidekickException("Could not fetch data from the trade API.");

        TextDictionary.Clear();
        FillDictionary(TextDictionary, result, x => x.Text);
    }

    private async Task InitializeInvariant(GameType game)
    {
        var cacheKey = $"{game.GetValueAttribute()}_InvariantData";
        var result = await cacheProvider.GetOrSet(cacheKey, () => tradeApiClient.FetchData<StaticItemCategory>(game, gameLanguageProvider.InvariantLanguage, "static"), (cache) => cache.Result.Any());
        if (result == null) throw new SidekickException("Could not fetch invariant data from the trade API.");

        InvariantDictionary.Clear();
        FillDictionary(InvariantDictionary, result, x => x.Id);
    }

    private void FillDictionary(Dictionary<string, StaticItem> dictionary, FetchResult<StaticItemCategory> result, Func<StaticItem, string?> keyFunc)
    {
        foreach (var category in result.Result)
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

    public StaticItem? GetById(string? id)
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

    public StaticItem? Get(string? name, string? type)
    {
        var data = !string.IsNullOrEmpty(name) ? TextDictionary.GetValueOrDefault(name) : null;
        data ??= !string.IsNullOrEmpty(type) ? TextDictionary.GetValueOrDefault(type) : null;
        if (data == null) return null;

        return GetById(data.Id);
    }

}
