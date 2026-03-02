using System.Text.RegularExpressions;
namespace Sidekick.Data.Builder.Pseudo.Definitions;

public class ElementalResistancesDefinition : PseudoDefinitionBuilder
{
    public override string StatId => "pseudo.pseudo_total_elemental_resistance";

    public override List<PseudoPattern> Patterns =>
    [
        new(new Regex("to (?:Fire|Cold|Lightning) Resistance$")),
        new(new Regex("to (?:Fire|Cold|Lightning) and (?:Fire|Cold|Lightning) Resistances$"), 2),
        new(new Regex("(?=.*Chaos)to (?:Fire|Cold|Lightning|Chaos) and (?:Fire|Cold|Lightning|Chaos) Resistances$")),
        new(new Regex("to all Elemental Resistances$"), 3),
    ];

    public override Regex Exception => new("Passive|While|Graft|Exposure|Minions|Enemies|Totems");
}
