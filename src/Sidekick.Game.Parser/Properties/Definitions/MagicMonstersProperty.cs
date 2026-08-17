using System.Text.RegularExpressions;
using Sidekick.Apis.Poe.Trade.Filters.AutoSelect;
using Sidekick.Apis.Poe.Trade.Filters.Types;
using Sidekick.Apis.Poe.Trade.Parser;
using Sidekick.Apis.Poe.Trade.Parser.Properties;
using Sidekick.Apis.Poe.Trade.Trade.Requests;
using Sidekick.Apis.Poe.Trade.Trade.Requests.Filters;
using Sidekick.Common.Enums;
using Sidekick.Common.Settings.Languages;
using Sidekick.Game.Parser.Items;
using ItemProperties = Sidekick.Game.Parser.Items.ItemProperties;

namespace Sidekick.Game.Parser.Properties.Definitions;

public class MagicMonstersProperty(
    GameType game,
    ICurrentGameLanguage currentGameLanguage) : PropertyDefinition
{
    private Regex Pattern { get; } = currentGameLanguage.Language.DescriptionMagicMonsters.ToRegexIntProperty();

    private Regex IsAugmentedPattern { get; } = currentGameLanguage.Language.DescriptionMagicMonsters.ToRegexIsAugmented();

    public override string Label => currentGameLanguage.Language.DescriptionMagicMonsters;

    public override void Parse(Item item)
    {
        item.Properties.MagicMonsters = GetInt(Pattern, item.Text);
        if (item.Properties.MagicMonsters == 0) return;

        if (GetBool(IsAugmentedPattern, item.Text)) item.Properties.AugmentedProperties.Add(nameof(ItemProperties.MagicMonsters));
    }

    public override Task<TradeFilter?> GetFilter(Item item)
    {
        if (item.Properties.MagicMonsters <= 0) return Task.FromResult<TradeFilter?>(null);

        var filter = new MagicMonstersFilter
        {
            Text = Label,
            Value = item.Properties.MagicMonsters,
            ValuePrefix = "+",
            ValueSuffix = "%",
            Augmented = item.Properties.AugmentedProperties.Contains(nameof(ItemProperties.MagicMonsters)),
            AutoSelectSettingKey = $"Trade_Filter_{nameof(MagicMonstersProperty)}_{game.GetValueAttribute()}",
            NormalizeEnabled = true,
        };
        return Task.FromResult<TradeFilter?>(filter);
    }
}

public class MagicMonstersFilter : IntPropertyFilter
{
    public MagicMonstersFilter()
    {
        DefaultAutoSelect = AutoSelectPreferences.Create(false);
    }

    public override void PrepareTradeRequest(Query query, Item item)
    {
        if (!Checked) return;

        query.Filters.GetOrCreateMapFilters().Filters.MagicMonsters = new StatFilterValue(this);
    }
}
