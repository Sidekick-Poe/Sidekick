using Sidekick.Data.Builder.Repoe.Models.Poe1;
using Sidekick.Data.Items.Models;

namespace Sidekick.Data.Builder.Repoe;

public class RepoeDataProvider(DataProvider dataProvider)
{
    public async Task<List<RepoeStatTranslation>> GetStatTranslations(GameType gameType, string language)
    {
        var files = gameType == GameType.PathOfExile1 ? RepoeDownloader.Poe1Files : RepoeDownloader.Poe2Files;
        var result = new List<RepoeStatTranslation>();
        foreach (var file in files)
        {
            result.AddRange(await dataProvider.Read<List<RepoeStatTranslation>>(gameType, $"repoe/{file.FileName}.{language}.json"));
        }

        return result;
    }
}