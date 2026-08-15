using Sidekick.Common.Initialization;
using Sidekick.Common.Settings;
using Sidekick.Common.Settings.Languages;
using Sidekick.Game.ItemDefinitions;
namespace Sidekick.Game.Providers;

public class ItemDefinitionProvider(
    DataProvider dataProvider,
    ICurrentGameLanguage currentGameLanguage,
    ISettingsService settingsService,
    BaseItemProvider baseItemProvider
) : IInitializableService
{
    public Dictionary<string, ItemDefinition> InvariantDictionary { get; } = new(StringComparer.Ordinal);

    public List<ItemDefinition> Definitions { get; private set; } = [];
    public List<ItemDefinition> InvariantDefinitions { get; private set; } = [];
    public List<ItemDefinition> UniqueItems { get; private set; } = [];

    public async Task Initialize()
    {
        var game = await settingsService.GetGame();

        Definitions = await dataProvider.Read<List<ItemDefinition>>(game, GameDataType.Items, currentGameLanguage.Language);
        AssignBaseItems(Definitions);
        UniqueItems = Definitions.Where(x => x.IsUnique)
            .OrderByDescending(x => x.Name?.Length ?? 0)
            .ToList();

        if (currentGameLanguage.IsEnglish())
        {
            InvariantDefinitions = Definitions;
        }
        else
        {
            InvariantDefinitions = await dataProvider.Read<List<ItemDefinition>>(game, GameDataType.Items, currentGameLanguage.InvariantLanguage);
            AssignBaseItems(InvariantDefinitions);
        }

        InvariantDictionary.Clear();
        foreach (var definition in InvariantDefinitions)
        {
            if (definition.UniqueIds != null)
            {
                foreach (var key in definition.UniqueIds)
                {
                    InvariantDictionary.TryAdd(key, definition);
                }
            }

            if (definition.BaseItemIds != null)
            {
                foreach (var key in definition.BaseItemIds)
                {
                    InvariantDictionary.TryAdd(key, definition);
                }
            }
        }

        return;

        void AssignBaseItems(List<ItemDefinition> definitions)
        {
            foreach (var definition in definitions)
            {
                if (definition.BaseItemIds == null) continue;
                definition.BaseItems = definition.BaseItemIds
                    .Select(x => baseItemProvider.Dictionary.GetValueOrDefault(x))
                    .Where(x => x != null)
                    .ToList()!;
            }
        }
    }
}
