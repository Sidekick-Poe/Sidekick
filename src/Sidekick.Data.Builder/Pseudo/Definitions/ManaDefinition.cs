using System.Text.RegularExpressions;
using Sidekick.Data.Items;
namespace Sidekick.Data.Builder.Pseudo.Definitions;

public class ManaDefinition(GameType game) : PseudoDefinitionBuilder
{
    public override string StatId => "pseudo.pseudo_total_mana";

    public override List<PseudoPattern> Patterns =>
    [
        new(new Regex("to maximum Mana$")),
        new(new Regex("to Intelligence$"), AttributeMultiplier),
        new(new Regex("(?=.*Intelligence)to (?:Strength|Dexterity|Intelligence) and (?:Strength|Dexterity|Intelligence)$"), AttributeMultiplier),
        new(new Regex("to all Attributes$"), AttributeMultiplier),
    ];

    public override Regex Exception => new("Passive|Zombies|Transformed");

    private double AttributeMultiplier => game == GameType.PathOfExile1 ? 0.5 : 2;
}
