using System.Text.RegularExpressions;
using Sidekick.Common.Game;

namespace Sidekick.Apis.Poe.Parser.Pseudo.Definitions;

public class ElementalResistancesDefinition(GameType game) : PseudoDefinition
{
    protected override bool Enabled => game == GameType.PathOfExile;

    protected override string? ModifierId => game == GameType.PathOfExile ? "pseudo.pseudo_total_elemental_resistance" : null;

    protected override List<PseudoPattern> Patterns =>
    [
        new(new Regex("to (?:Fire|Cold|Lightning) Resistance$")),
        new(new Regex("to (?:Fire|Cold|Lightning) and (?:Fire|Cold|Lightning) Resistances$"), 2),
        new(new Regex("(?=.*Chaos)to (?:Fire|Cold|Lightning|Chaos) and (?:Fire|Cold|Lightning|Chaos) Resistances$")),
        new(new Regex("to all Elemental Resistances$"), 3),
    ];

    protected override Regex Exception => new("Minions|Enemies|Totems");
}
