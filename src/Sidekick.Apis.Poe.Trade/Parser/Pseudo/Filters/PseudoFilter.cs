using Sidekick.Apis.Poe.Items;
using Sidekick.Apis.Poe.Trade.Parser.Filters;
namespace Sidekick.Apis.Poe.Trade.Parser.Pseudo.Filters;

public class PseudoFilter : IAutoSelectableFilter
{
    public required PseudoModifier PseudoModifier { get; set; }

    public bool? Checked { get; set; } = false;

    public double? Min { get; set; }

    public double? Max { get; set; }

    public string Text => PseudoModifier.Text ?? string.Empty;
}
