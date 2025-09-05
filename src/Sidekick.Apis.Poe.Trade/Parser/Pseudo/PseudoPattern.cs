using System.Text.RegularExpressions;

namespace Sidekick.Apis.Poe.Trade.Parser.Pseudo;

public class PseudoPattern
(
    Regex regex,
    double multiplier = 1
)
{
    public Regex Pattern { get; } = regex;

    public double Multiplier { get; } = multiplier;
}
