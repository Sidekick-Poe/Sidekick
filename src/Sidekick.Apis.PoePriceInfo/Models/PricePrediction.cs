namespace Sidekick.Apis.PoePriceInfo.Models;

/// <summary>
/// Contains the result of a PricePredictionQuery
/// </summary>
public class PricePrediction
{
    /// <summary>
    /// The minimum estimated value of the item
    /// </summary>
    public double Min { get; set; }

    /// <summary>
    /// The maximum estimated value of the item
    /// </summary>
    public double Max { get; set; }

    /// <summary>
    /// The currency in which the item is evaluated
    /// </summary>
    public string? Currency { get; set; }

    /// <summary>
    /// How confident is the prediction. Higher is better.
    /// </summary>
    public double ConfidenceScore { get; set; }
}
