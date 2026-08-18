using Sidekick.Common.Settings;
using Sidekick.Game.Parser.Filters.AutoSelect;
using Sidekick.Game.Parser.Items;
namespace Sidekick.Game.Parser.Filters.Types;

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
