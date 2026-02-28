using System.Text.RegularExpressions;
using Microsoft.Extensions.Localization;
using Sidekick.Apis.Poe.Extensions;
using Sidekick.Apis.Poe.Items;
using Sidekick.Apis.Poe.Trade.Localization;
using Sidekick.Apis.Poe.Trade.Trade.Filters.Types;
using Sidekick.Common.Enums;
using Sidekick.Common.Settings;
using Sidekick.Data;
using Sidekick.Data.Languages;
using Sidekick.Data.Pseudo;

namespace Sidekick.Apis.Poe.Trade.Parser.Pseudo;

public class PseudoParser
(
    ISettingsService settingsService,
    IStringLocalizer<PoeResources> resources,
    DataProvider dataProvider,
    ICurrentGameLanguage currentGameLanguage
) : IPseudoParser
{
    private static readonly Regex ParseHashPattern = new(@"\#");

    private List<PseudoDefinition> Definitions { get; set; } = [];

    /// <inheritdoc/>
    public int Priority => 300;

    /// <inheritdoc/>
    public async Task Initialize()
    {
        var game = await settingsService.GetGame();
        Definitions = await dataProvider.Read<List<PseudoDefinition>>(game, DataType.Pseudo, currentGameLanguage.Language);
    }

    public void Parse(Item item)
    {
        item.PseudoStats.Clear();
        foreach (var definition in Definitions)
        {
            var result = GetItemPseudoStat(definition, item.Stats);
            if (result != null && !string.IsNullOrEmpty(result.Text)) item.PseudoStats.Add(result);
        }

        return;

        ItemPseudoStat? GetItemPseudoStat(PseudoDefinition definition, List<Stat> itemStats)
        {
            var value = 0d;
            foreach (var itemStat in itemStats)
            {
                foreach (var definitionStat in definition.Stats)
                {
                    if (itemStat.Definitions.All(x => !x.TradeIds.Contains(definitionStat.Id))) continue;

                    value += itemStat.AverageValue * definitionStat.Multiplier;
                    break;
                }
            }

            if (value == 0) return null;

            var text = GetText(definition.Text, value);
            if (string.IsNullOrEmpty(text)) return null;

            return new ItemPseudoStat
            {
                Id = definition.PseudoStatId,
                Text = definition.Text ?? string.Empty,
                Value = (int)value,
            };
        }

        string GetText(string? text, double value)
        {
            if (text == null) return string.Empty;

            return ParseHashPattern
                .Replace(text, ((int)value).ToString(), 1)
                .Replace("+-", "-");
        }
    }

    public async Task<List<TradeFilter>> GetFilters(Item item)
    {
        if (!ItemClassConstants.WithStats.Contains(item.Properties.ItemClass)) return [];

        var result = new List<TradeFilter>();
        foreach (var stat in item.PseudoStats)
        {
            result.Add(new PseudoFilter(stat)
            {
                AutoSelectSettingKey = $"Trade_Filter_Pseudo_{item.Game.GetValueAttribute()}_{stat.Id}",
            });
        }

        var expandableFilter = new ExpandableFilter(resources["Pseudo_Filters"], result.ToArray())
        {
            Checked = true,
        };
        await expandableFilter.Initialize(item, settingsService);
        expandableFilter.Checked = true;

        return [expandableFilter];
    }
}
