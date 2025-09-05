using Sidekick.Apis.Poe.Trade.Parser.Pseudo.Definitions;
using Sidekick.Common.Extensions;
using Sidekick.Common.Game.Items;
using Sidekick.Common.Settings;

namespace Sidekick.Apis.Poe.Trade.Parser.Pseudo;

public class PseudoParser
(
    IServiceProvider serviceProvider,
    ISettingsService settingsService
) : IPseudoParser
{
    private List<PseudoDefinition> Definitions { get; } = new();

    /// <inheritdoc/>
    public int Priority => 300;

    /// <inheritdoc/>
    public async Task Initialize()
    {
        var leagueId = await settingsService.GetString(SettingKeys.LeagueId);
        var game = leagueId.GetGameFromLeagueId();

        Definitions.Clear();
        Definitions.AddRange([
            new ElementalResistancesDefinition(game),
            new ChaosResistancesDefinition(game),
            new StrengthDefinition(game),
            new IntelligenceDefinition(game),
            new DexterityDefinition(game),
            new LifeDefinition(game),
            new ManaDefinition(game),
        ]);

        foreach (var definition in Definitions)
        {
            definition.InitializeDefinition(serviceProvider);
        }
    }

    public List<PseudoModifier> Parse(List<ModifierLine> lines)
    {
        var results = new List<PseudoModifier>();

        foreach (var definition in Definitions)
        {
            var result = definition.Parse(lines);
            if (result != null) results.Add(result);
        }

        return results;
    }
}
