using System.Text.RegularExpressions;
using Sidekick.Common.Game;

namespace Sidekick.Apis.Poe.Parser.Pseudo.Definitions;

public class ManaDefinition(GameType game) : PseudoDefinition
{
    protected override bool Enabled => game == GameType.PathOfExile;

    protected override string? ModifierId => game == GameType.PathOfExile ? "pseudo.pseudo_total_mana" : null;

    protected override List<PseudoPattern> Patterns =>
    [
        new PseudoPattern(new Regex("to maximum Mana$")),
        new PseudoPattern(new Regex("to Intelligence$"), AttributeMultiplier),
        new PseudoPattern(new Regex("(?=.*Intelligence)to (?:Strength|Dexterity|Intelligence) and (?:Strength|Dexterity|Intelligence)$"), AttributeMultiplier),
        new PseudoPattern(new Regex("to all Attributes$"), AttributeMultiplier),
    ];

    protected override Regex Exception => new("Zombies|Transformed");

    private double AttributeMultiplier => game == GameType.PathOfExile ? 0.5 : 2;
}
