using System.Text.RegularExpressions;
using Sidekick.Apis.Poe.Extensions;
using Sidekick.Apis.Poe.Items;
using Sidekick.Apis.Poe.Languages;
using Sidekick.Apis.Poe.Trade.ApiStatic;
using Sidekick.Common.Settings;
using Sidekick.Data.Trade;
using Sidekick.Data.Trade.Models.Raw;

namespace Sidekick.Apis.Poe.Trade.ApiItems;

public class ApiItemProvider
(
    TradeDataProvider tradeDataProvider,
    ICurrentGameLanguage currentGameLanguage,
    ISettingsService settingsService,
    IApiStaticDataProvider apiStaticDataProvider
) : IApiItemProvider
{
    public List<(Regex Regex, ItemApiInformation Item)> NamePatterns { get; private set; } = [];

    public List<(Regex Regex, ItemApiInformation Item)> TypePatterns { get; private set; } = [];

    public List<(Regex Regex, ItemApiInformation Item)> TextPatterns { get; private set; } = [];

    public List<ItemApiInformation> UniqueItems { get; private set; } = [];

    /// <inheritdoc/>
    public int Priority => 200;

    /// <inheritdoc/>
    public async Task Initialize()
    {
        var game = await settingsService.GetGame();
        var result = await tradeDataProvider.GetItems(game, currentGameLanguage.Language.Code);

        InitializeItems(result);
        UniqueItems = NamePatterns.Select(x => x.Item)
            .Where(x => x.IsUnique)
            .OrderByDescending(x => x.Name?.Length)
            .ToList();

        TypePatterns = TypePatterns.OrderByDescending(x => x.Item.Type?.Length ?? 0).ToList();
        TextPatterns = TextPatterns.OrderByDescending(x => x.Item.Text?.Length ?? 0).ToList();
    }

    private void InitializeItems(List<RawTradeItemCategory> result)
    {
        NamePatterns.Clear();
        TypePatterns.Clear();
        TextPatterns.Clear();

        foreach (var category in result)
        {
            foreach (var entry in category.Entries)
            {
                var information = entry.ToItemApiInformation();
                information.Category = category.Id;

                var apiData = apiStaticDataProvider.Get(information.Name, information.Type);
                information.InvariantId = apiData?.Id;
                information.InvariantText = apiData?.Text;
                information.Image = apiData?.Image;

                if (currentGameLanguage.IsEnglish())
                {
                    information.InvariantName = entry.Name;
                    information.InvariantType = entry.Type;
                    if (string.IsNullOrEmpty(information.InvariantText)) information.InvariantText = entry.Text;
                }

                if (!string.IsNullOrEmpty(information.Name))
                {
                    var regex = $"^{Regex.Escape(information.Name)}|{Regex.Escape(information.Name)}$";
                    NamePatterns.Add((new Regex(regex), information));
                }

                if (!string.IsNullOrEmpty(information.Type) && !information.IsUnique)
                {
                    // Match as a whole "word" (not inside other words like "Clashing")
                    // \p{L} = any unicode letter, so this works across languages too.
                    var typeRegex = $@"(?<!\p{{L}}){Regex.Escape(information.Type)}(?!\p{{L}})";
                    TypePatterns.Add((new Regex(typeRegex, RegexOptions.Compiled), information));
                }

                if (!string.IsNullOrEmpty(information.Text))
                {
                    TextPatterns.Add((new Regex(Regex.Escape(information.Text)), information));
                }
            }
        }
    }
}
