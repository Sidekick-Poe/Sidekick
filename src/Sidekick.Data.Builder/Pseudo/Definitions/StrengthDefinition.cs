using System.Text.RegularExpressions;
namespace Sidekick.Data.Builder.Pseudo.Definitions;

public class StrengthDefinition : PseudoDefinitionBuilder
{
    public override string StatId => "pseudo.pseudo_total_strength";

    public override List<PseudoPattern> Patterns =>
    [
        new(new Regex("to Strength$")),
        new(new Regex("(?=.*Strength)to (?:Strength|Dexterity|Intelligence) and (?:Strength|Dexterity|Intelligence)$")),
        new(new Regex("to all Attributes$")),
    ];

    public override Regex Exception => new("Passive");
}
