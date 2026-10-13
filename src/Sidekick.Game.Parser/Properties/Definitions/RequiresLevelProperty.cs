using System.Text.RegularExpressions;
using Sidekick.Common.Enums;
using Sidekick.Game.Parser.Filters.AutoSelect;
using Sidekick.Game.Parser.Filters.Types;
using Sidekick.Game.Parser.Items;
using Sidekick.Game.Parser.Trade.Requests;
using Sidekick.Game.Parser.Trade.Requests.Filters;
using Sidekick.Game.Providers;
namespace Sidekick.Game.Parser.Properties.Definitions;

public class RequiresLevelProperty(
    GameType game,
    GameTextProvider gameTextProvider) : PropertyDefinition
{
    private Regex Pattern { get; } = gameTextProvider.Texts.ItemPropertyLevel.ToRegexIntProperty();

    private Regex RequiresPattern { get; } = new($@"^{gameTextProvider.Texts.ItemPropertyRequires}.*{gameTextProvider.Texts.ItemPropertyLevel}\s*(\d+)");

    public override string Label => gameTextProvider.Texts.ItemPropertyLevel;

    public override void Parse(Item item)
    {
        if (item.ItemClass.IsGem()) return;

        var block = item.Text.Blocks.FirstOrDefault(x => x.Text.StartsWith(gameTextProvider.Texts.ItemPropertyRequires, StringComparison.InvariantCultureIgnoreCase));
        block ??= item.Text.Blocks.FirstOrDefault(x => x.Text.StartsWith(gameTextProvider.Texts.ItemPropertyRequirements, StringComparison.InvariantCultureIgnoreCase));
        if (block == null) return;

        item.Properties.RequiresLevel = GetInt(Pattern, block);
        if (item.Properties.RequiresLevel == 0) item.Properties.RequiresLevel = GetInt(RequiresPattern, block);
        if (item.Properties.RequiresLevel == 0) return;

        block.Parsed = true;
    }

    public override Task<TradeFilter?> GetFilter(Item item)
    {
        if (item.Properties.RequiresLevel <= 0) return Task.FromResult<TradeFilter?>(null);

        var filter = new RequiresLevelFilter
        {
            Text = Label,
            Value = item.Properties.RequiresLevel,
            AutoSelectSettingKey = $"Trade_Filter_{nameof(RequiresLevelProperty)}_{game.GetValueAttribute()}",
            NormalizeEnabled = false,
        };
        return Task.FromResult<TradeFilter?>(filter);
    }
}

public class RequiresLevelFilter : IntPropertyFilter
{
    public RequiresLevelFilter()
    {
        DefaultAutoSelect = AutoSelectPreferences.Create(false, normalizeBy: 0);
    }

    public override void PrepareTradeRequest(Query query, Item item)
    {
        if (!Checked) return;

        query.Filters.GetOrCreateRequirementsFilters().Filters.Level = new StatFilterValue(this);
    }
}
