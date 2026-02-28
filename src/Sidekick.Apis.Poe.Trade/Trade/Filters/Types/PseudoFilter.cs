using Sidekick.Apis.Poe.Items;
using Sidekick.Apis.Poe.Trade.Trade.Filters.AutoSelect;
using Sidekick.Apis.Poe.Trade.Trade.Items.Requests;
using Sidekick.Apis.Poe.Trade.Trade.Items.Requests.Filters;
using Sidekick.Common.Settings;
namespace Sidekick.Apis.Poe.Trade.Trade.Filters.Types;

public class PseudoFilter : TradeFilter, INormalizableFilter
{
    public PseudoFilter(ItemPseudoStat stat)
    {
        Stat = stat;
        Text = stat.Text;

        DefaultAutoSelect = AutoSelectPreferences.Create(false);
    }

    public override async Task<AutoSelectResult?> Initialize(Item item, ISettingsService settingsService)
    {
        var result = await base.Initialize(item, settingsService);
        if (result == null) return null;

        if (result.FillMinRange) Min = ((INormalizableFilter)this).NormalizeMinValue(result.NormalizeBy);
        if (result.FillMaxRange) Max = ((INormalizableFilter)this).NormalizeMaxValue(result.NormalizeBy);

        return result;
    }

    public ItemPseudoStat Stat { get; init; }

    public double? Min { get; set; }

    public double? Max { get; set; }

    public double NormalizeValue => Stat.Value;

    public bool NormalizeEnabled => true;

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
