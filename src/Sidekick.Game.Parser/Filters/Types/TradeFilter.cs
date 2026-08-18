using Sidekick.Common.Settings;
using Sidekick.Game.Parser.Filters.AutoSelect;
using Sidekick.Game.Parser.Items;
using Sidekick.Game.Parser.Trade.Requests;
namespace Sidekick.Game.Parser.Filters.Types;

public abstract class TradeFilter
{
    public virtual async Task<AutoSelectResult?> Initialize(Item item, ISettingsService settingsService)
    {
        if (string.IsNullOrEmpty(AutoSelectSettingKey)) return null;

        CustomAutoSelect = await settingsService.GetObject<AutoSelectPreferences>(AutoSelectSettingKey);
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

    public bool Augmented { get; init; }

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
}
