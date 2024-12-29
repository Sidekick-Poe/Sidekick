using System.Text.RegularExpressions;

namespace Sidekick.Apis.Poe.Parser.Pseudo;

public class PseudoPattern
(
    Regex regex,
    int multiplier = 1
)
{
    public Regex Pattern { get; set; } = regex;

    public int Multiplier { get; set; } = multiplier;
}
