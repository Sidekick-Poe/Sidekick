namespace Sidekick.Apis.Poe.Trade.Parser.Filters;

/// <summary>
/// Standard interface for trade filters that can be auto-selected on overlay open.
/// Implemented by <see cref="Pseudo.Filters.PseudoFilter"/>,
/// <see cref="Modifiers.ModifierFilter"/> and <see cref="Properties.Filters.PropertyFilter"/>.
/// </summary>
public interface IAutoSelectableFilter
{
    /// <summary>
    /// Gets or sets whether this filter is initially checked.
    /// </summary>
    bool? Checked { get; set; }

    /// <summary>
    /// User-visible text representing this filter.
    /// Used by rule engine (e.g., Regex matching).
    /// </summary>
    string Text { get; }
}
