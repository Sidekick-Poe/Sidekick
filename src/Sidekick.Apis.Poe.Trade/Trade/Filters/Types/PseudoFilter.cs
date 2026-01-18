using Sidekick.Apis.Poe.Items;
using Sidekick.Apis.Poe.Trade.Trade.Filters.AutoSelect;
using Sidekick.Apis.Poe.Trade.Trade.Items.Requests;
using Sidekick.Apis.Poe.Trade.Trade.Items.Requests.Filters;
using Sidekick.Common.Settings;
namespace Sidekick.Apis.Poe.Trade.Trade.Filters.Types;

public class PseudoFilter : TradeFilter
{
    public PseudoFilter(PseudoStat stat)
    {
        Stat = stat;
        Text = stat.Text;

        DefaultAutoSelect = AutoSelectPreferences.Create(false);
    }

    public override async Task<AutoSelectResult?> Initialize(Item item, ISettingsService settingsService)
    {
        var result = await base.Initialize(item, settingsService);
        if (result == null) return null;

        if (result.FillMinRange) NormalizeMinValue(result.NormalizeBy);
        if (result.FillMaxRange) NormalizeMaxValue(result.NormalizeBy);

        return result;
    }

    public PseudoStat Stat { get; init; }

    public double? Min { get; set; }

    public double? Max { get; set; }

    private void NormalizeMinValue(double normalizeBy)
    {
        if (Stat.Value == 0)
        {
            Min = Stat.Value;
            return;
        }

        if (Stat.Value > 0)
        {
            var normalizedValue = (1 - normalizeBy) * Stat.Value;
            Min = Math.Round(normalizedValue, 2);
        }
        else
        {
            var normalizedValue = (1 + normalizeBy) * Stat.Value;
            Min = Math.Round(normalizedValue, 2);
        }
    }

    private void NormalizeMaxValue(double normalizeby)
    {
        if (Stat.Value == 0)
        {
            Max = Stat.Value;
            return;
        }

        if (Stat.Value > 0)
        {
            var normalizedValue = (1 + normalizeby) * Stat.Value;
            Max = Math.Round(normalizedValue, 2);
        }
        else
        {
            var normalizedValue = (1 - normalizeby) * Stat.Value;
            Max = Math.Round(normalizedValue, 2);
        }
    }

    public override void PrepareTradeRequest(Query query, Item item)
    {
        if (!Checked || string.IsNullOrEmpty(Stat.Id))
        {
            return;
        }

        query.GetOrCreateStatGroup(StatType.And).Filters.Add(new StatFilters()
        {
            Id = Stat.Id,
            Value = new StatFilterValue()
            {
                Min = Min,
                Max = Max,
            },
        });
    }
}
