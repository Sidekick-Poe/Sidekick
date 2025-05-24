using System.Text.RegularExpressions;
using Sidekick.Common.Game;

namespace Sidekick.Apis.Poe.Trade.Parser.Pseudo.Definitions;

public class DexterityDefinition(GameType game) : PseudoDefinition
{
    protected override bool Enabled => game == GameType.PathOfExile;

    protected override string? ModifierId => game == GameType.PathOfExile ? "pseudo.pseudo_total_dexterity" : null;

    protected override List<PseudoPattern> Patterns =>
    [
        new(new Regex("to Dexterity$")),
        new(new Regex("(?=.*Dexterity)to (?:Strength|Dexterity|Intelligence) and (?:Strength|Dexterity|Intelligence)$")),
        new(new Regex("to all Attributes$")),
    ];

    protected override Regex Exception => new("Passive");
}
