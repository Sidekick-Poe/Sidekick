using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Sidekick.Common;
using Sidekick.Common.Enums;
using Sidekick.Data.Builder.Repoe;
using Sidekick.Data.ItemClasses;
using Sidekick.Data.ItemDefinitions;
using Sidekick.Data.Languages;
namespace Sidekick.Data.Builder.ItemClasses;

public class ItemClassBuilder(
    ILogger<ItemClassBuilder> logger,
    IOptions<SidekickConfiguration> configuration,
    DataProvider dataProvider,
    RepoeDownloader repoeDownloader)
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
        var raw = await repoeDownloader.ReadItemClasses(game, language.Code);
        var result = new List<ItemClassDefinition>();

        foreach (var entry in raw)
        {
            result.Add(new ItemClassDefinition()
            {
                Id = entry.Key,
                Name = entry.Value.Name,
                Type = EnumExtensions.FindValue<ItemClass>(itemClass => itemClass.FindAttributes<ItemClassGameId>().Any(attr => attr.Id == entry.Key && attr.Game == game)),
            });
        }

        await dataProvider.Write(game, DataType.ItemClasses, language, result);
    }
}
