using System.Text.RegularExpressions;
namespace Sidekick.Data.Builder.Pseudo.Definitions;

public class ChaosResistancesDefinition : PseudoDefinitionBuilder
{
    public override string StatId => "pseudo.pseudo_total_chaos_resistance";

    public override List<PseudoPattern> Patterns =>
    [
        new(new Regex("to Chaos Resistance$")),
        new(new Regex("(?=.*Chaos)to (?:Fire|Cold|Lightning|Chaos) and (?:Fire|Cold|Lightning|Chaos) Resistances$")),
    ];

    public override Regex Exception => new("While|Passive|Minions|Enemies|Totems");
}
