using System.Text.RegularExpressions;

namespace Sidekick.Apis.Poe.Trade.Parser.Pseudo.Definitions;

public class ElementalResistancesDefinition : PseudoDefinition
{
    protected override bool Enabled => true;

    protected override string ModifierId => "pseudo.pseudo_total_elemental_resistance";

    protected override List<PseudoPattern> Patterns =>
    [
        new(new Regex("to (?:Fire|Cold|Lightning) Resistance$")),
        new(new Regex("to (?:Fire|Cold|Lightning) and (?:Fire|Cold|Lightning) Resistances$"), 2),
        new(new Regex("(?=.*Chaos)to (?:Fire|Cold|Lightning|Chaos) and (?:Fire|Cold|Lightning|Chaos) Resistances$")),
        new(new Regex("to all Elemental Resistances$"), 3),
    ];

    protected override Regex Exception => new("Minions|Enemies|Totems");
}
