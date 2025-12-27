using System.Text.RegularExpressions;
using Sidekick.Apis.Poe.Items;
using Sidekick.Apis.Poe.Languages;
using Sidekick.Apis.Poe.Trade.ApiItems;
using Sidekick.Apis.Poe.Trade.Trade.Filters.AutoSelect;
using Sidekick.Apis.Poe.Trade.Trade.Filters.Types;
using Sidekick.Apis.Poe.Trade.Trade.Items.Requests;
using Sidekick.Apis.Poe.Trade.Trade.Items.Requests.Filters;
using Sidekick.Apis.Poe.Trade.Trade.Items.Results;
using Sidekick.Common.Enums;
using Sidekick.Common.Settings;

namespace Sidekick.Apis.Poe.Trade.Parser.Properties.Definitions;

public class RewardProperty(
    GameType game,
    ISettingsService settingsService,
    IGameLanguageProvider gameLanguageProvider,
    IApiItemProvider apiItemProvider) : PropertyDefinition
{
    private Regex Pattern { get; } = gameLanguageProvider.Language.DescriptionReward.ToRegexStringCapture();

    public override List<ItemClass> ValidItemClasses { get; } = [
        ItemClass.Map,
    ];

    public override void Parse(Item item)
    {
        if (game == GameType.PathOfExile2) return;

        var propertyBlock = item.Text.Blocks[1];
        item.Properties.Reward = GetString(Pattern, propertyBlock);
        if (item.Properties.Reward != null) propertyBlock.Parsed = true;
    }

    public override async Task<TradeFilter?> GetFilter(Item item)
    {
        if (game == GameType.PathOfExile2 || item.Properties.Reward == null) return null;

        var autoSelectKey = $"Trade_Filter_{nameof(RewardProperty)}_{game.GetValueAttribute()}";
        var filter = new RewardFilter(apiItemProvider)
        {
            Text = gameLanguageProvider.Language.DescriptionReward,
            Value = item.Properties.Reward!,
            Type = LineContentType.Unique,
            AutoSelectSettingKey = autoSelectKey,
            AutoSelect = await settingsService.GetObject<AutoSelectPreferences>(autoSelectKey, () => null),
        };
        return filter;
    }
}

public class RewardFilter : StringPropertyFilter
{
    public RewardFilter(IApiItemProvider apiItemProvider)
    {
        ApiItemProvider = apiItemProvider;
        DefaultAutoSelect = new AutoSelectPreferences()
        {
            Mode = AutoSelectMode.Always,
        };
    }

    private IApiItemProvider ApiItemProvider { get; }

    public override void PrepareTradeRequest(Query query, Item item)
    {
        if (!Checked) return;

        var uniqueItem = ApiItemProvider.UniqueItems.FirstOrDefault(x => x.Name != null && Value.Contains(x.Name));
        if (uniqueItem?.Name == null) return;

        query.Filters.GetOrCreateMapFilters().Filters.Reward = new SearchFilterOption(uniqueItem.Name);
    }
}
