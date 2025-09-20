using System.Text.RegularExpressions;
using Sidekick.Apis.Poe.Models;

namespace Sidekick.Apis.Poe.Trade.Parser.Pseudo.Definitions;

public class LifeDefinition(GameType game) : PseudoDefinition
{
    protected override bool Enabled => true;

    protected override string? ModifierId => "pseudo.pseudo_total_life";

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
