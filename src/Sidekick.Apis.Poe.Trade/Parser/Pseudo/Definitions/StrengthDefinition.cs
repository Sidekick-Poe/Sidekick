using System.Text.RegularExpressions;
using Sidekick.Apis.Poe.Models;

namespace Sidekick.Apis.Poe.Trade.Parser.Pseudo.Definitions;

public class StrengthDefinition : PseudoDefinition
{
    protected override bool Enabled => true;

    protected override string? ModifierId => "pseudo.pseudo_total_strength";

    protected override List<PseudoPattern> Patterns =>
    [
        new(new Regex("to Strength$")),
        new(new Regex("(?=.*Strength)to (?:Strength|Dexterity|Intelligence) and (?:Strength|Dexterity|Intelligence)$")),
        new(new Regex("to all Attributes$")),
    ];

    protected override Regex Exception => new("Passive");
}
