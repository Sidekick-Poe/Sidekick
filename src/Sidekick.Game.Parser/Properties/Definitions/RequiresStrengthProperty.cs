using System.Text.RegularExpressions;
using Sidekick.Common.Enums;
using Sidekick.Game.Parser.Filters.AutoSelect;
using Sidekick.Game.Parser.Filters.Types;
using Sidekick.Game.Parser.Items;
using Sidekick.Game.Parser.Trade.Requests;
using Sidekick.Game.Parser.Trade.Requests.Filters;
using Sidekick.Game.Providers;
namespace Sidekick.Game.Parser.Properties.Definitions;

public class RequiresStrengthProperty(
    GameType game,
    GameTextProvider gameTextProvider) : PropertyDefinition
{
    private Regex Pattern { get; } = gameTextProvider.Texts.ItemPropertyStrengthShort.ToRegexIntProperty();

    private Regex RequiresPattern { get; } = new($@"^{gameTextProvider.Texts.ItemPropertyRequires}.*?(\d+)(?:\ \([a-z]+\))?\s*{gameTextProvider.Texts.ItemPropertyStrengthShort}");

    public override string Label => gameTextProvider.Texts.ItemPropertyStrength;

    public override void Parse(Item item)
    {
        if (item.ItemClass.IsGem()) return;

        var block = item.Text.Blocks.FirstOrDefault(x => x.Text.StartsWith(gameTextProvider.Texts.ItemPropertyRequires, StringComparison.InvariantCultureIgnoreCase));
        block ??= item.Text.Blocks.FirstOrDefault(x => x.Text.StartsWith(gameTextProvider.Texts.ItemPropertyRequirements, StringComparison.InvariantCultureIgnoreCase));
        if (block == null) return;

        item.Properties.RequiresStrength = GetInt(Pattern, block);
        if (item.Properties.RequiresStrength == 0) item.Properties.RequiresStrength = GetInt(RequiresPattern, block);
        if (item.Properties.RequiresStrength == 0) return;

        block.Parsed = true;
    }

    public override Task<TradeFilter?> GetFilter(Item item)
    {
        if (item.Properties.RequiresStrength <= 0) return Task.FromResult<TradeFilter?>(null);

        var filter = new RequiresStrengthFilter
        {
            Text = Label,
            Value = item.Properties.RequiresStrength,
            AutoSelectSettingKey = $"Trade_Filter_{nameof(RequiresStrengthProperty)}_{game.GetValueAttribute()}",
            NormalizeEnabled = false,
        };
        return Task.FromResult<TradeFilter?>(filter);
    }
}

public class RequiresStrengthFilter : IntPropertyFilter
{
    public RequiresStrengthFilter()
    {
        DefaultAutoSelect = AutoSelectPreferences.Create(false, normalizeBy: 0);
    }

    public override void PrepareTradeRequest(Query query, Item item)
    {
        if (!Checked) return;

        query.Filters.GetOrCreateRequirementsFilters().Filters.Strength = new StatFilterValue(this);
    }
}
