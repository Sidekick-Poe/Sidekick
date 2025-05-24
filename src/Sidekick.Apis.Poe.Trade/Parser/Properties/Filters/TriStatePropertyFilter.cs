namespace Sidekick.Apis.Poe.Trade.Parser.Properties.Filters;

public class TriStatePropertyFilter : BooleanPropertyFilter
{
    internal TriStatePropertyFilter(
        PropertyDefinition definition) : base(definition)
    {
    }

    public new bool? Checked { get; set; }
}
