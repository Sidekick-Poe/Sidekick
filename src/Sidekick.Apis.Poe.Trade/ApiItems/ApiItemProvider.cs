using Sidekick.Apis.Poe.Extensions;
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
    private Dictionary<string, ItemDefinition> IdDictionary { get; } = new(StringComparer.Ordinal);
    private Dictionary<string, ItemDefinition> TextDictionary { get; } = new(StringComparer.Ordinal);

    public List<ItemDefinition> Definitions { get; private set; } = [];
    public List<ItemDefinition> UniqueItems { get; private set; } = [];

    public int Priority => 100;

    public async Task Initialize()
    {
        var game = await settingsService.GetGame();

        Definitions = await dataProvider.Read<List<ItemDefinition>>(game, DataType.Items, currentGameLanguage.Language);

        UniqueItems = Definitions.Where(x => x.IsUnique)
            .OrderByDescending(x => x.Name?.Length ?? 0)
            .ToList();

        TextDictionary.Clear();
        IdDictionary.Clear();
        foreach (var definition in Definitions)
        {
            if (!string.IsNullOrEmpty(definition.Id)) IdDictionary.TryAdd(definition.Id, definition);

            if (!string.IsNullOrEmpty(definition.Name)) TextDictionary.TryAdd(definition.Name, definition);
            if (!string.IsNullOrEmpty(definition.Type)) TextDictionary.TryAdd(definition.Type, definition);
        }
    }

    public ItemDefinition? GetById(string? id)
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

    public ItemDefinition? Get(string? name, string? type)
    {
        var data = !string.IsNullOrEmpty(name) ? TextDictionary.GetValueOrDefault(name) : null;
        data ??= !string.IsNullOrEmpty(type) ? TextDictionary.GetValueOrDefault(type) : null;
        if (data == null) return null;

        return GetById(data.Id);
    }
}
