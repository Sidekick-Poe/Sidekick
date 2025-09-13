using System.Text.RegularExpressions;
using Sidekick.Apis.Poe.Models;

namespace Sidekick.Apis.Poe.Trade.Parser.Pseudo.Definitions;

public class ChaosResistancesDefinition(GameType game) : PseudoDefinition
{
    protected override bool Enabled => game == GameType.PathOfExile;

    protected override string? ModifierId => game == GameType.PathOfExile ? "pseudo.pseudo_total_chaos_resistance" : null;

    protected override List<PseudoPattern> Patterns =>
    [
        new(new Regex("to Chaos Resistance$")),
        new(new Regex("(?=.*Chaos)to (?:Fire|Cold|Lightning|Chaos) and (?:Fire|Cold|Lightning|Chaos) Resistances$")),
    ];

    protected override Regex Exception => new("Minions|Enemies|Totems");
}
