using Sidekick.Apis.Poe.Items;
namespace Sidekick.Apis.Poe.Trade.Parser.Pseudo.Filters;

public class PseudoFilter
{
    public required PseudoModifier PseudoModifier { get; set; }

    public bool? Checked { get; set; } = false;

    public double? Min { get; set; }

    public double? Max { get; set; }
}
