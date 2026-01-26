namespace Sidekick.Apis.Poe.Trade.Trade.Filters.AutoSelect;

public class AutoSelectComparisonAttribute(params AutoSelectComparisonType[] allowedComparisons) : Attribute
{
    public AutoSelectComparisonType[] AllowedComparisons { get; } = allowedComparisons;
}
