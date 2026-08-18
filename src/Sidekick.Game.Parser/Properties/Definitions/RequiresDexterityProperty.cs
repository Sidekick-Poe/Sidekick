using System.Text.RegularExpressions;
using Sidekick.Common.Enums;
using Sidekick.Common.Settings.Languages;
using Sidekick.Game.Parser.Filters.AutoSelect;
using Sidekick.Game.Parser.Filters.Types;
using Sidekick.Game.Parser.Items;
using Sidekick.Game.Parser.Trade.Requests;
using Sidekick.Game.Parser.Trade.Requests.Filters;
namespace Sidekick.Game.Parser.Properties.Definitions;

public class RequiresDexterityProperty(
    GameType game,
    ICurrentGameLanguage currentGameLanguage) : PropertyDefinition
{
    private Regex Pattern { get; } = currentGameLanguage.Language.DescriptionRequiresDex.ToRegexIntProperty();

    private Regex RequiresPattern { get; } = new($@"^{currentGameLanguage.Language.DescriptionRequires}.*?(\d+)(?:\ \([a-z]+\))?\s*{currentGameLanguage.Language.DescriptionRequiresDex}");

    public override string Label => currentGameLanguage.Language.DescriptionRequiresDex;

    public override void Parse(Item item)
    {
        if (item.ItemClass.IsGem()) return;

        var block = item.Text.Blocks.FirstOrDefault(x => x.Text.StartsWith(currentGameLanguage.Language.DescriptionRequires, StringComparison.InvariantCultureIgnoreCase));
        block ??= item.Text.Blocks.FirstOrDefault(x => x.Text.StartsWith(currentGameLanguage.Language.DescriptionRequirements, StringComparison.InvariantCultureIgnoreCase));
        if (block == null) return;

        item.Properties.RequiresDexterity = GetInt(Pattern, block);
        if (item.Properties.RequiresDexterity == 0) item.Properties.RequiresDexterity = GetInt(RequiresPattern, block);
        if (item.Properties.RequiresDexterity == 0) return;

        block.Parsed = true;
    }

    public override Task<TradeFilter?> GetFilter(Item item)
    {
        if (item.Properties.RequiresDexterity <= 0) return Task.FromResult<TradeFilter?>(null);

        var filter = new RequiresDexterityFilter
        {
            Text = Label,
            Value = item.Properties.RequiresDexterity,
            AutoSelectSettingKey = $"Trade_Filter_{nameof(RequiresDexterityProperty)}_{game.GetValueAttribute()}",
            NormalizeEnabled = false,
        };
        return Task.FromResult<TradeFilter?>(filter);
    }
}

public class RequiresDexterityFilter : IntPropertyFilter
{
    public RequiresDexterityFilter()
    {
        DefaultAutoSelect = AutoSelectPreferences.Create(false, normalizeBy: 0);
    }

    public override void PrepareTradeRequest(Query query, Item item)
    {
        if (!Checked) return;

        query.Filters.GetOrCreateRequirementsFilters().Filters.Dexterity = new StatFilterValue(this);
    }
}
