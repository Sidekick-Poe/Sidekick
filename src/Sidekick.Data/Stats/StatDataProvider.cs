using Sidekick.Data.Items.Models;
using Sidekick.Data.Stats.Models;
namespace Sidekick.Data.Stats;

public class StatDataProvider(DataProvider dataProvider)
{
    public async Task<List<StatDefinition>> GetStats(GameType gameType, string language)
    {
        return await dataProvider.Read<List<StatDefinition>>(gameType, $"stats/{language}.json");
    }
}