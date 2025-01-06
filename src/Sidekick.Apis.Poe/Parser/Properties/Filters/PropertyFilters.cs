namespace Sidekick.Apis.Poe.Parser.Properties.Filters;

public class PropertyFilters
{
    public bool BaseTypeFilterApplied { get; set; } = true;
    public bool ClassFilterApplied { get; set; }
    public bool RarityFilterApplied { get; set; }
    public List<BooleanPropertyFilter> Filters { get; set; } = new();
}
