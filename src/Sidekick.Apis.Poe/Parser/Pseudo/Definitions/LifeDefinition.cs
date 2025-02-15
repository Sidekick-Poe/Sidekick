using System.Text.RegularExpressions;
using Sidekick.Common.Game;

namespace Sidekick.Apis.Poe.Parser.Pseudo.Definitions;

public class LifeDefinition(GameType game) : PseudoDefinition
{
    protected override bool Enabled => game == GameType.PathOfExile;

    protected override string? ModifierId => game == GameType.PathOfExile ? "pseudo.pseudo_total_life" : null;

    protected override List<PseudoPattern> Patterns =>
    [
        new(new Regex("to maximum Life$")),
        new(new Regex("to Strength$"), AttributeMultiplier),
        new(new Regex("(?=.*Strength)to (?:Strength|Dexterity|Intelligence) and (?:Strength|Dexterity|Intelligence)$"), AttributeMultiplier),
        new(new Regex("to all Attributes$"), AttributeMultiplier),
    ];

    protected override Regex Exception => new("Zombies|Transformed");

    private double AttributeMultiplier => game == GameType.PathOfExile ? 0.5 : 2;
}
