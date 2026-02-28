using System.Text.RegularExpressions;
using Sidekick.Data.Items;

namespace Sidekick.Apis.Poe.Trade.Parser.Pseudo.Definitions;

public class ManaDefinition(GameType game) : PseudoDefinition
{
    protected override bool Enabled => true;

    protected override string StatId => "pseudo.pseudo_total_mana";

    protected override List<PseudoPattern> Patterns =>
    [
        new PseudoPattern(new Regex("to maximum Mana$")),
        new PseudoPattern(new Regex("to Intelligence$"), AttributeMultiplier),
        new PseudoPattern(new Regex("(?=.*Intelligence)to (?:Strength|Dexterity|Intelligence) and (?:Strength|Dexterity|Intelligence)$"), AttributeMultiplier),
        new PseudoPattern(new Regex("to all Attributes$"), AttributeMultiplier),
    ];

    protected override Regex Exception => new("Zombies|Transformed");

    private double AttributeMultiplier => game == GameType.PathOfExile1 ? 0.5 : 2;
}
