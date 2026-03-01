using System.Text.RegularExpressions;
namespace Sidekick.Data.Builder.Pseudo.Definitions;

public class IntelligenceDefinition : PseudoDefinitionBuilder
{
    public override string StatId => "pseudo.pseudo_total_intelligence";

    public override List<PseudoPattern> Patterns =>
    [
        new(new Regex("to Intelligence$")),
        new(new Regex("(?=.*Intelligence)to (?:Strength|Dexterity|Intelligence) and (?:Strength|Dexterity|Intelligence)$")),
        new(new Regex("to all Attributes$")),
    ];

    public override Regex Exception => new("Passive");
}
