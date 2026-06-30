using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Sidekick.Common;
using Sidekick.Data.Builder.Trade.Models;
using Sidekick.Data.Languages;
using Sidekick.Data.Stats;
using Sidekick.Data.StatsInvariant;
using Sidekick.Data.Trade;
namespace Sidekick.Data.Builder.StatsInvariant;

public class StatsInvariantBuilder
(
    ILogger<StatsInvariantBuilder> logger,
    IOptions<SidekickConfiguration> configuration,
    IGameLanguageProvider gameLanguageProvider,
    DataProvider dataProvider
)
{
    public async Task Build(IGameLanguage language)
    {
        if (language.Code != gameLanguageProvider.InvariantLanguage.Code) return;

        try
        {
            await BuildForGame(GameType.PathOfExile1, language);
            await BuildForGame(GameType.PathOfExile2, language);
        }
        catch (Exception ex)
        {
            if (configuration.Value.ApplicationType == SidekickApplicationType.DataBuilder || configuration.Value.ApplicationType == SidekickApplicationType.Test)
            {
                throw;
            }

            logger.LogError(ex, "Failed to build invariant trade stats.");
        }
    }

    private async Task BuildForGame(GameType game, IGameLanguage language)
    {
        var definitions = await dataProvider.Read<List<TradeStatDefinition>>(game, DataType.TradeStats, language);

        var model = new StatsInvariantDetails()
        {
            IgnoreStatIds = GetIgnoreStatIds(definitions).ToList(),
            FireWeaponDamageIds = GetFireWeaponDamageIds(definitions).ToList(),
            ColdWeaponDamageIds = GetColdWeaponDamageIds(definitions).ToList(),
            LightningWeaponDamageIds = GetLightningWeaponDamageIds(definitions).ToList(),
            IncursionRoomStatIds = GetIncursionRooms(definitions).ToList(),
            LogbookFactionStatIds = GetLogbookFactions(definitions).ToList(),
            LogbookBossStatIds = GetLogbookBosses(definitions).ToList(),
        };

        await dataProvider.Write(game, DataType.StatsInvariant, model);
    }

    private IEnumerable<string> GetIgnoreStatIds(List<TradeStatDefinition> definitions)
    {
        foreach (var definition in definitions)
        {
            if (definition.Id.GetStatCategory() != StatCategory.Pseudo) continue;
            if (definition.Text.StartsWith("#% chance for dropped Maps to convert to")) yield return definition.Id;
        }
    }

    private IEnumerable<string> GetFireWeaponDamageIds(List<TradeStatDefinition> definitions)
    {
        foreach (var definition in definitions)
        {
            if (definition.Id.GetStatCategory() == StatCategory.Pseudo) continue;
            if (definition.Text == "Adds # to # Fire Damage") yield return definition.Id;
        }
    }

    private IEnumerable<string> GetColdWeaponDamageIds(List<TradeStatDefinition> definitions)
    {
        foreach (var definition in definitions)
        {
            if (definition.Id.GetStatCategory() == StatCategory.Pseudo) continue;
            if (definition.Text == "Adds # to # Cold Damage") yield return definition.Id;
        }
    }

    private IEnumerable<string> GetLightningWeaponDamageIds(List<TradeStatDefinition> definitions)
    {
        foreach (var definition in definitions)
        {
            if (definition.Id.GetStatCategory() == StatCategory.Pseudo) continue;
            if (definition.Text == "Adds # to # Lightning Damage") yield return definition.Id;
        }
    }

    private IEnumerable<string> GetIncursionRooms(List<TradeStatDefinition> definitions)
    {
        foreach (var definition in definitions)
        {
            if (definition.Id.GetStatCategory() != StatCategory.Pseudo) continue;
            if (definition.Text.StartsWith("Has Room: ") && definition.Id.GetStatOption() != 2) yield return definition.Id;
        }
    }

    private IEnumerable<string> GetLogbookFactions(List<TradeStatDefinition> definitions)
    {
        foreach (var definition in definitions)
        {
            if (definition.Id.GetStatCategory() != StatCategory.Pseudo) continue;
            if (definition.Text.StartsWith("Has Logbook Faction: ")) yield return definition.Id;
        }
    }

    private IEnumerable<string> GetLogbookBosses(List<TradeStatDefinition> definitions)
    {
        foreach (var definition in definitions)
        {
            if (definition.Id.GetStatCategory() != StatCategory.Implicit) continue;
            if (definition.Text == "Area contains an Expedition Boss (#)") yield return definition.Id;
        }
    }
}
