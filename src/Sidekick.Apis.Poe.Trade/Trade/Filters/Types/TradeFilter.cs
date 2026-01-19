using Sidekick.Apis.Poe.Items;
using Sidekick.Apis.Poe.Trade.Trade.Filters.AutoSelect;
using Sidekick.Apis.Poe.Trade.Trade.Items.Requests;
using Sidekick.Apis.Poe.Trade.Trade.Items.Results;
using Sidekick.Common.Settings;
namespace Sidekick.Apis.Poe.Trade.Trade.Filters.Types;

public abstract class TradeFilter
{
    public virtual async Task<AutoSelectResult?> Initialize(Item item, ISettingsService settingsService)
    {
        if (string.IsNullOrEmpty(AutoSelectSettingKey)) return null;

        CustomAutoSelect = await settingsService.GetObject<AutoSelectPreferences>(AutoSelectSettingKey, () => null);
        CustomAutoSelect??= new AutoSelectPreferences()
        {
            Mode = AutoSelectMode.Default,
        };

        if (AutoSelect == null)
        {
            Checked = false;
            return null;
        }

        var result = await AutoSelect.GetResult(item, this, settingsService);
        if (this is TriStatePropertyFilter triStateFilter)
        {
            triStateFilter.Checked = result.Checked;
        }
        else
        {
            Checked = result.Checked ?? false;
        }

        return result;
    }

    public virtual bool Checked { get; set; }

    public string Text { get; init; } = string.Empty;

    public string? Hint { get; init; }

    public LineContentType Type { get; init; } = LineContentType.Simple;

    public string? AutoSelectSettingKey { get; init; }

    public AutoSelectPreferences? AutoSelect
    {
        get
        {
            if (CustomAutoSelect?.Mode == AutoSelectMode.Default) return DefaultAutoSelect;
            return CustomAutoSelect ?? DefaultAutoSelect;
        }
    }

    public AutoSelectPreferences? CustomAutoSelect { get; set; }

    public AutoSelectPreferences? DefaultAutoSelect { get; init; }

    public virtual void PrepareTradeRequest(Query query, Item item) {}

    public virtual Task OnChanged() => Task.CompletedTask;
}
