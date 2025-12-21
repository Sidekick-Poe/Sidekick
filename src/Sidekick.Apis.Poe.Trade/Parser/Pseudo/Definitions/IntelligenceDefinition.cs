using System.Text.RegularExpressions;

namespace Sidekick.Apis.Poe.Trade.Parser.Pseudo.Definitions;

public class IntelligenceDefinition : PseudoDefinition
{
    protected override bool Enabled => true;

    protected override string StatId => "pseudo.pseudo_total_intelligence";

    protected override List<PseudoPattern> Patterns =>
    [
        new(new Regex("to Intelligence$")),
        new(new Regex("(?=.*Intelligence)to (?:Strength|Dexterity|Intelligence) and (?:Strength|Dexterity|Intelligence)$")),
        new(new Regex("to all Attributes$")),
    ];

    protected override Regex Exception => new("Passive");
}
