using System.Text.RegularExpressions;

namespace Sidekick.Apis.Poe.Trade.Parser.Pseudo.Definitions;

public class DexterityDefinition : PseudoDefinition
{
    protected override bool Enabled => true;

    protected override string? ModifierId => "pseudo.pseudo_total_dexterity";

    protected override List<PseudoPattern> Patterns =>
    [
        new(new Regex("to Dexterity$")),
        new(new Regex("(?=.*Dexterity)to (?:Strength|Dexterity|Intelligence) and (?:Strength|Dexterity|Intelligence)$")),
        new(new Regex("to all Attributes$")),
    ];

    protected override Regex Exception => new("Passive");
}
