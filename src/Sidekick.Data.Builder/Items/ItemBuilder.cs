using System.Text.RegularExpressions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Sidekick.Common;
using Sidekick.Data.Builder.Trade.Models;
using Sidekick.Data.Items;
using Sidekick.Data.Languages;
namespace Sidekick.Data.Builder.Items;

public class ItemBuilder(
    ILogger<ItemBuilder> logger,
    IOptions<SidekickConfiguration> configuration,
    IGameLanguageProvider languageProvider,
    DataProvider dataProvider)
{
    public async Task Build(IGameLanguage language)
    {
        try
        {
            await BuildForGame(GameType.PathOfExile1, language);
            await BuildForGame(GameType.PathOfExile2, language);
        }
        catch (Exception ex)
        {
            if (configuration.Value.ApplicationType == SidekickApplicationType.DataBuilder || configuration.Value.ApplicationType == SidekickApplicationType.Test)
            {
                throw;
            }

            logger.LogError(ex, "Failed to build items data.");
        }
    }

    private async Task BuildForGame(GameType game, IGameLanguage language)
    {
        var itemsResult = await dataProvider.Read<RawTradeResult<List<RawTradeItemCategory>>>(game, DataType.RawTradeItems, language);
        var invariantItems = await GetInvariantDictionary(game, language);

        var staticItems = await GetStaticDictionary(game, language);
        StaticItem? GetStatic(string? name, string? type)
        {
            var data = !string.IsNullOrEmpty(name) ? staticItems.FirstOrDefault(x => x.Text == name) : null;
            data ??= !string.IsNullOrEmpty(type) ? staticItems.FirstOrDefault(x => x.Text == type) : null;
            return data;
        }

        var list = new List<ItemDefinition>();
        foreach (var category in itemsResult.Result)
        {
            foreach (var entry in category.Entries)
            {
                var staticItem = GetStatic(entry.Name, entry.Type);
                var text = staticItem?.Text ?? entry.Text;
                if (text == entry.Name || text == entry.Type) text = null;

                var item = new ItemDefinition
                {
                    Id = staticItem?.Id,
                    Image = staticItem?.Image,
                    Name = entry.Name,
                    Type = entry.Type,
                    Text = text,
                    Category = category.Id,
                    IsUnique = entry.IsUnique,
                    Discriminator = entry.Discriminator,
                };

                FillInvariant(item);
                FillPatterns(item);

                list.Add(item);
            }
        }

        await dataProvider.Write(game, DataType.Items, language, list);

        return;

        void FillInvariant(ItemDefinition item)
        {
            if (language.Code == languageProvider.InvariantLanguage.Code)
            {
                item.InvariantText = item.Text;
                item.InvariantName = item.Name;
                item.InvariantType = item.Type;
                return;
            }

            if (invariantItems == null) return;

            var invariant = invariantItems.GetValueOrDefault(item.Id ?? string.Empty);
            if (invariant != null)
            {
                item.InvariantText = invariant.Text;
                item.InvariantName = invariant.Name;
                item.InvariantType = invariant.Type;
            }
        }

        void FillPatterns(ItemDefinition item)
        {
            if (!string.IsNullOrEmpty(item.Name))
            {
                var regex = $"^{Regex.Escape(item.Name)}|{Regex.Escape(item.Name)}$";
                item.NamePattern = new Regex(regex);
            }

            if (!string.IsNullOrEmpty(item.Text))
            {
                item.TextPattern = new Regex(Regex.Escape(item.Text));
            }

            if (!item.IsUnique && !string.IsNullOrEmpty(item.Type))
            {
                var regex = $@"(?<!\p{{L}}){Regex.Escape(item.Type)}(?!\p{{L}})";
                item.TypePattern = new Regex(regex);
            }
        }
    }

    private record StaticItem(string Id, string? Text, string? Image);

    private async Task<List<StaticItem>> GetStaticDictionary(GameType game, IGameLanguage language)
    {
        var raw = await dataProvider.Read<RawTradeResult<List<RawTradeStaticItemCategory>>>(game, DataType.RawTradeStatic, language);
        var result = new List<StaticItem>();

        foreach (var category in raw.Result)
        {
            foreach (var entry in category.Entries)
            {
                if (entry.Id == null! || entry.Text == null || entry.Id == "sep") continue;

                var image = string.IsNullOrEmpty(entry.Image) ? null : $"https://web.poecdn.com{entry.Image}";
                var item = new StaticItem(entry.Id, entry.Text, image);
                result.Add(item);
            }
        }

        return result;
    }

    private async Task<Dictionary<string, ItemDefinition>?> GetInvariantDictionary(GameType game, IGameLanguage language)
    {
        if (language.Code == languageProvider.InvariantLanguage.Code) return null;

        var raw = await dataProvider.Read<List<ItemDefinition>>(game, DataType.Items, languageProvider.InvariantLanguage);
        return raw.Where(x => x.Id != null)
            .DistinctBy(x => x.Id)
            .ToDictionary(x => x.Id!, x => x);
    }
}
