using Sidekick.Apis.Poe.Trade.Filters.AutoSelect;
using Sidekick.Common.Settings;
using Sidekick.Data.Items;
namespace Sidekick.Apis.Poe.Trade.Filters.Types;

public class OptionFilter(string? settingKey) : TradeFilter
{
    public record OptionFilterItem(string? Value, string? Text);

    public string? Value { get; set; }

    public virtual string? DefaultValue { get; init; }

    public override bool Checked => Value != DefaultValue;

    public required List<OptionFilterItem> Options { get; init; }

    public override async Task<AutoSelectResult?> Initialize(Item item, ISettingsService settingsService)
    {
        Value = DefaultValue;
        if (!string.IsNullOrEmpty(settingKey))
        {
            var settingValue = await settingsService.GetString(settingKey);
            if (settingValue != null) Value = settingValue;
        }

        return await base.Initialize(item, settingsService);
    }

    public async Task OnChanged(ISettingsService settingsService)
    {
        if (!string.IsNullOrEmpty(settingKey))
        {
            await settingsService.Set(settingKey, Value);
        }
    }
}
