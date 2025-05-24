namespace Sidekick.Apis.Poe.Trade.Parser.Properties.Filters;

public class PropertyFilters
{
    public bool BaseTypeFilterApplied { get; set; }
    public bool ClassFilterApplied { get; set; }
    public bool RarityFilterApplied { get; set; }
    public List<BooleanPropertyFilter> Filters { get; set; } = new();
}
