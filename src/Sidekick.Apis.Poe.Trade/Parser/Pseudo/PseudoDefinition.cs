using System.Text.RegularExpressions;
using Sidekick.Apis.Poe.Items;
using Sidekick.Data.Trade.Models;
using Sidekick.Data.Trade.Models.Raw;

namespace Sidekick.Apis.Poe.Trade.Parser.Pseudo;

public abstract class PseudoDefinition
{
    private static readonly Regex ParseHashPattern = new("\\#");

    protected abstract bool Enabled { get; }

    protected abstract string StatId { get; }

    protected abstract List<PseudoPattern> Patterns { get; }

    /// <summary>
    /// Represents a regular expression pattern used to exclude certain stat texts
    /// during the processing of pseudo-stat definitions in the Path of Exile API.
    /// </summary>
    /// <remarks>
    /// This property defines a regular expression that matches stat texts
    /// which should be excluded from further processing. Each derived class provides
    /// a specific implementation of this property. It is utilized within the
    /// initialization and parsing processes to filter out unwanted stat entries.
    /// </remarks>
    protected abstract Regex? Exception { get; }

    private string? Text { get; set; }

    private List<PseudoStatDefinition> Definitions { get; set; } = new();

    internal void InitializeDefinition(List<RawTradeStatCategory> apiCategories, List<TradeStatDefinition>? localizedPseudoStats)
    {
        if (!Enabled) return;

        foreach (var apiStat in apiCategories.SelectMany(apiCategory => apiCategory.Entries))
        {
            if (Exception != null && Exception.IsMatch(apiStat.Text)) continue;

            foreach (var pattern in Patterns)
            {
                if (!pattern.Pattern.IsMatch(apiStat.Text)) continue;

                Definitions.Add(new PseudoStatDefinition(apiStat.Id, apiStat.Text, pattern.Multiplier));
            }
        }

        Definitions = Definitions.OrderBy(x => x.Text).ToList();
        Text = localizedPseudoStats?.FirstOrDefault(x => x.Id == StatId)?.Text ?? null;
    }

    internal PseudoStat? Parse(List<Stat> lines)
    {
        if (!Enabled || !HasPseudoMods(lines))
        {
            return null;
        }

        var result = new PseudoStat
        {
            Id = StatId,
            Text = Text ?? string.Empty,
        };

        foreach (var line in lines)
        {
            foreach (var definition in Definitions)
            {
                if (line.Definitions.All(x => !x.TradeIds.Contains(definition.Id))) continue;

                result.Value += line.AverageValue * definition.Multiplier;
                break;
            }
        }

        result.Value = (int)result.Value;
        result.Text = ParseHashPattern
            .Replace(result.Text, ((int)result.Value).ToString(), 1)
            .Replace("+-", "-");
        return result;
    }

    private bool HasPseudoMods(List<Stat> lines)
    {
        foreach (var definition in Definitions)
        {
            foreach (var line in lines)
            {
                foreach (var lineDefinitions in line.Definitions)
                {
                    if (lineDefinitions.TradeIds.Contains(definition.Id)) return true;
                }
            }
        }

        return false;
    }
}
