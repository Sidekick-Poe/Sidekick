using Sidekick.Data.Items.Models;
using Sidekick.Data.Stats.Models;
namespace Sidekick.Apis.Poe.Items;

/// <summary>
///     Represents a line of text on an item. With the API being the way it is, each line of text can be represented by one
///     or more api modifiers.
/// </summary>
public class Stat(StatCategory category, string text)
{
    /// <summary>
    ///     Gets or sets the original line of text as it is in the game.
    /// </summary>
    public string Text { get; } = text;

    public StatCategory Category { get; } = category;

    private IReadOnlyList<StatMatchedPattern> matchedPatterns = [];
    public IReadOnlyList<StatMatchedPattern> MatchedPatterns
    {
        get => matchedPatterns;
        set
        {
            tradePatterns = null;
            matchedPatterns = value;
        }
    }

    private IReadOnlyList<TradeStatPattern>? tradePatterns;
    public IReadOnlyList<TradeStatPattern> TradePatterns
    {
        get
        {
            tradePatterns ??= GetTradePatterns().ToList();
            return tradePatterns;

            IEnumerable<TradeStatPattern> GetTradePatterns()
            {
                var handledIds = new List<string>();
                foreach (var matchedPattern in MatchedPatterns)
                {
                    if (matchedPattern.GamePattern?.Option != null)
                    {
                        var tradePattern = matchedPattern.Definition.TradePatterns.FirstOrDefault(x => x.Option?.Id == matchedPattern.GamePattern.Option);
                        if (tradePattern != null)
                        {
                            handledIds.Add(tradePattern.Id);
                            yield return tradePattern;
                        }
                    }
                    else if (matchedPattern.GamePattern != null)
                    {
                        foreach (var tradePattern in matchedPattern.Definition.TradePatterns)
                        {
                            if (handledIds.Contains(tradePattern.Id)) continue;

                            if (matchedPattern.GamePattern.Option != null)
                            {
                                if (matchedPattern.GamePattern.Option == tradePattern.Option?.Id)
                                {
                                    handledIds.Add(tradePattern.Id);
                                    yield return tradePattern;
                                }
                            }
                            else if (tradePattern.Category == StatCategory.Explicit || tradePattern.Category == Category)
                            {
                                handledIds.Add(tradePattern.Id);
                                yield return tradePattern;
                            }
                        }
                    }

                    if (matchedPattern.TradePattern != null && !handledIds.Contains(matchedPattern.TradePattern.Id))
                    {
                        if (matchedPattern.TradePattern.Category == StatCategory.Explicit || matchedPattern.TradePattern.Category == Category)
                        {
                            handledIds.Add(matchedPattern.TradePattern.Id);
                            yield return matchedPattern.TradePattern;
                        }
                    }
                }
            }
        }
    }

    public bool HasMultipleCategories => TradePatterns.DistinctBy(x => x.Category).Count() > 1;

    /// <summary>
    ///     Gets or sets a list of values on this modifier line.
    /// </summary>
    public List<double> Values { get; set; } = [];

    /// <summary>
    /// Gets the average value of the numerical values associated with the modifier line.
    /// If there are no values, returns 0.
    /// </summary>
    public double AverageValue => Values.Count > 0 ? Values.Average() : 0;

    /// <summary>
    ///     Gets a value indicating whether this modifier has double values.
    /// </summary>
    public bool HasValues => TradePatterns.All(x => x.Option == null) && Values.Count > 0;

    public int BlockIndex { get; init; }

    public int LineIndex { get; init; }

    [Obsolete]
    public bool MatchedFuzzily { get; }

    /// <inheritdoc />
    public override string ToString() => Text;
}
