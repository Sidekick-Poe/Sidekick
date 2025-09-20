using System.Text.RegularExpressions;

namespace Sidekick.Apis.Poe.Trade.Parser.Pseudo.Definitions;

public class ChaosResistancesDefinition : PseudoDefinition
{
    protected override bool Enabled => true;

    protected override string? ModifierId => "pseudo.pseudo_total_chaos_resistance";

    protected override List<PseudoPattern> Patterns =>
    [
        new(new Regex("to Chaos Resistance$")),
        new(new Regex("(?=.*Chaos)to (?:Fire|Cold|Lightning|Chaos) and (?:Fire|Cold|Lightning|Chaos) Resistances$")),
    ];

    protected override Regex Exception => new("Minions|Enemies|Totems");
}
