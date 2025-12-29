using Sidekick.Apis.Poe.Items;
using Sidekick.Apis.Poe.Trade.Trade.Filters.AutoSelect;
using Sidekick.Apis.Poe.Trade.Trade.Items.Requests;
using Sidekick.Apis.Poe.Trade.Trade.Items.Results;
namespace Sidekick.Apis.Poe.Trade.Trade.Filters.Types;

public abstract class TradeFilter
{
    public void Initialize(Item item)
    {
        var autoSelect = AutoSelect ?? DefaultAutoSelect;
        if (autoSelect != null)
        {
            var result = autoSelect.ShouldCheck(item, this);
            if (this is TriStatePropertyFilter triStateFilter)
            {
                triStateFilter.Checked = result;
            }
            else
            {
                Checked = result ?? false;
            }
        }
        else
        {
            Checked = false;
        }
    }

    public virtual bool Checked { get; set; }

    public string Text { get; init; } = string.Empty;

    public string? Hint { get; init; }

    public LineContentType Type { get; init; } = LineContentType.Simple;

    public string? AutoSelectSettingKey { get; init; }

    public AutoSelectPreferences? AutoSelect { get; init; }

    public AutoSelectPreferences? DefaultAutoSelect { get; init; }

    public virtual void PrepareTradeRequest(Query query, Item item) {}

    public virtual Task OnChanged() => Task.CompletedTask;
}
