using Sidekick.Apis.Poe.Items;
using Sidekick.Apis.Poe.Trade.Trade.Filters.AutoSelect;
using Sidekick.Apis.Poe.Trade.Trade.Items.Requests;
using Sidekick.Apis.Poe.Trade.Trade.Items.Results;
namespace Sidekick.Apis.Poe.Trade.Trade.Filters.Types;

public class TradeFilter
{
    public virtual bool Checked { get; set; }

    public string Text { get; set; } = string.Empty;

    public string? Hint { get; init; }

    public LineContentType Type { get; init; } = LineContentType.Simple;

    public AutoSelectPreferences? AutoSelect { get; init; }
    public AutoSelectPreferences? DefaultAutoSelect { get; init; }
    public bool IsAutoSelectDefaults => !Equals(AutoSelect, DefaultAutoSelect);

    public virtual void PrepareTradeRequest(Query query, Item item) {}

    public virtual Task OnChanged() => Task.CompletedTask;
}
