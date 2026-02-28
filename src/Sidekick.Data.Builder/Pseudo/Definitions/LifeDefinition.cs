using System.Text.RegularExpressions;
using Sidekick.Data.Items;
namespace Sidekick.Data.Builder.Pseudo.Definitions;

public class LifeDefinition(GameType game) : PseudoDefinitionBuilder
{
    public override string StatId => "pseudo.pseudo_total_life";

    public override List<PseudoPattern> Patterns =>
    [
        new(new Regex("to maximum Life$")),
        new(new Regex("to Strength$"), AttributeMultiplier),
        new(new Regex("(?=.*Strength)to (?:Strength|Dexterity|Intelligence) and (?:Strength|Dexterity|Intelligence)$"), AttributeMultiplier),
        new(new Regex("to all Attributes$"), AttributeMultiplier),
    ];

    public override Regex Exception => new("Passive|Zombies|Transformed");

    private double AttributeMultiplier => game == GameType.PathOfExile1 ? 0.5 : 2;
}
