using Sidekick.Apis.Poe.Items;
using Sidekick.Data.Builder.Repoe.Models.Poe1;

namespace Sidekick.Data.Builder.Repoe;

public class RepoeDataProvider(DataProvider dataProvider)
{
    public async Task<List<RepoeStatTranslation>> GetStatTranslations(GameType gameType, string language)
    {
        return await dataProvider.Read<List<RepoeStatTranslation>>(gameType, $"repoe/stat_translations.{language}.json");
    }
}