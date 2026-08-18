namespace Sidekick.Game.Parser.Filters.AutoSelect;

public class AutoSelectComparisonAttribute(params AutoSelectComparisonType[] allowedComparisons) : Attribute
{
    public AutoSelectComparisonType[] AllowedComparisons { get; } = allowedComparisons;
}
