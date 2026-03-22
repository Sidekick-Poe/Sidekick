using Sidekick.Apis.Poe.Extensions;
using Sidekick.Apis.Poe.Trade.Trade.Items.Models;
using Sidekick.Common.Settings;
using Sidekick.Data;
using Sidekick.Data.Items;
using Sidekick.Data.Languages;
namespace Sidekick.Apis.Poe.Trade.ApiItems;

public class ApiItemProvider(
    DataProvider dataProvider,
    ICurrentGameLanguage currentGameLanguage,
    ISettingsService settingsService
) : IApiItemProvider
{
    private Dictionary<string, ItemDefinition> TextDictionary { get; } = new(StringComparer.Ordinal);

    public ItemDefinition? ExaltedOrb { get; private set; }
    public ItemDefinition? ChaosOrb { get; private set; }
    public ItemDefinition? DivineOrb { get; private set; }

    public List<ItemDefinition> Definitions { get; private set; } = [];
    public Dictionary<string, ItemDefinition> InvariantDictionary { get; } = [];
    public List<ItemDefinition> UniqueItems { get; private set; } = [];

    public int Priority => 100;

    public async Task Initialize()
    {
        var game = await settingsService.GetGame();

        Definitions = await dataProvider.Read<List<ItemDefinition>>(game, DataType.Items, currentGameLanguage.Language);
        UniqueItems = Definitions.Where(x => x.UniqueItem != null)
            .OrderByDescending(x => x.UniqueItem?.Name?.Length ?? 0)
            .ToList();

        TextDictionary.Clear();
        foreach (var definition in Definitions)
        {
            if (!string.IsNullOrEmpty(definition.TradeItem?.Name)) TextDictionary.TryAdd(definition.TradeItem.Name, definition);
            if (!string.IsNullOrEmpty(definition.TradeItem?.Type)) TextDictionary.TryAdd(definition.TradeItem.Type, definition);

            switch (definition.TradeItem?.Id)
            {
                case "chaos":
                    ChaosOrb = definition;
                    break;
                case "exalted":
                case "exalt":
                    ExaltedOrb = definition;
                    break;
                case "divine":
                    DivineOrb = definition;
                    break;
            }
        }

        await BuildInvariantDictionary(game);
    }
    private async Task BuildInvariantDictionary(GameType game)
    {
        InvariantDictionary.Clear();
        // todo
        // if (currentGameLanguage.Language.Code == currentGameLanguage.InvariantLanguage.Code) return;

        var definitions = await dataProvider.Read<List<ItemDefinition>>(game, DataType.Items, currentGameLanguage.InvariantLanguage);
        foreach (var definition in definitions)
        {
            var key = definition.UniqueItem?.Name;
            key ??= definition.BaseItem?.Name;
            key ??= definition.TradeItem?.Id;
            if (string.IsNullOrEmpty(key)) continue;

            InvariantDictionary.Add(key, definition);
        }
    }

    public ItemDefinition? Get(ApiItem apiItem)
    {
        var data = !string.IsNullOrEmpty(apiItem.Name) ? TextDictionary.GetValueOrDefault(apiItem.Name) : null;
        data ??= !string.IsNullOrEmpty(apiItem.Type) ? TextDictionary.GetValueOrDefault(apiItem.Type) : null;
        return data?.TradeItem == null ? null : data;
    }
}
