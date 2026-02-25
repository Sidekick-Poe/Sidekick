using Sidekick.Data.Items.Models;
using Sidekick.Data.Stats.Models;
namespace Sidekick.Data.Stats;

public class StatDataProvider(DataProvider dataProvider)
{
    public async Task<List<ItemStatDefinition>> GetStats(GameType gameType, string language)
    {
        return await dataProvider.Read<List<ItemStatDefinition>>(gameType, $"items/stats.{language}.json");
    }
}