using System.Text.RegularExpressions;
namespace Sidekick.Data.Builder.Pseudo.Definitions;

public class DexterityDefinition : PseudoDefinitionBuilder
{
    public override string StatId => "pseudo.pseudo_total_dexterity";

    public override List<PseudoPattern> Patterns =>
    [
        new(new Regex("to Dexterity$")),
        new(new Regex("(?=.*Dexterity)to (?:Strength|Dexterity|Intelligence) and (?:Strength|Dexterity|Intelligence)$")),
        new(new Regex("to all Attributes$")),
    ];

    public override Regex Exception => new("Passive");
}
