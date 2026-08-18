using System.Text.RegularExpressions;
using Sidekick.Common.Enums;
using Sidekick.Common.Settings.Languages;
using Sidekick.Game.Parser.Filters.AutoSelect;
using Sidekick.Game.Parser.Filters.Types;
using Sidekick.Game.Parser.Items;
using Sidekick.Game.Parser.Trade.Requests;
using Sidekick.Game.Parser.Trade.Requests.Filters;
using ItemProperties = Sidekick.Game.Parser.Items.ItemProperties;

namespace Sidekick.Game.Parser.Properties.Definitions;

public class MonsterPackSizeProperty(
    GameType game,
    ICurrentGameLanguage currentGameLanguage) : PropertyDefinition
{
    private Regex Pattern { get; } = currentGameLanguage.Language.DescriptionMonsterPackSize.ToRegexIntProperty();

    private Regex IsAugmentedPattern { get; } = currentGameLanguage.Language.DescriptionMonsterPackSize.ToRegexIsAugmented();

    public override string Label => currentGameLanguage.Language.DescriptionMonsterPackSize;

    public override void Parse(Item item)
    {
        item.Properties.MonsterPackSize = GetInt(Pattern, item.Text);
        if (item.Properties.MonsterPackSize == 0) return;

        if (GetBool(IsAugmentedPattern, item.Text)) item.Properties.AugmentedProperties.Add(nameof(ItemProperties.MonsterPackSize));
    }

    public override Task<TradeFilter?> GetFilter(Item item)
    {
        if (item.Properties.MonsterPackSize <= 0) return Task.FromResult<TradeFilter?>(null);

        var filter = new MonsterPackSizeFilter
        {
            Text = Label,
            Value = item.Properties.MonsterPackSize,
            ValuePrefix = "+",
            ValueSuffix = "%",
            Augmented = item.Properties.AugmentedProperties.Contains(nameof(ItemProperties.MonsterPackSize)),
            AutoSelectSettingKey = $"Trade_Filter_{nameof(MonsterPackSizeProperty)}_{game.GetValueAttribute()}",
            NormalizeEnabled = true,
        };
        return Task.FromResult<TradeFilter?>(filter);
    }
}

public class MonsterPackSizeFilter : IntPropertyFilter
{
    public MonsterPackSizeFilter()
    {
        DefaultAutoSelect = AutoSelectPreferences.Create(false);
    }

    public override void PrepareTradeRequest(Query query, Item item)
    {
        if (!Checked) return;

        query.Filters.GetOrCreateMapFilters().Filters.MonsterPackSize = new StatFilterValue(this);
    }
}
