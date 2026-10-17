using System.Collections.Concurrent;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Sidekick.Common;
using Sidekick.Game.ItemClasses;
using Sidekick.Game.Providers;
using Sidekick.Game.Providers.Languages.Implementations;
namespace Sidekick.Game.Parser.Tests;

/// <summary>
///     Reads the game data files from the repository's data folder, the same way the application does in test mode.
/// </summary>
public static class GameData
{
    private static readonly DataProvider DataProvider = new(
        Options.Create(new SidekickConfiguration { ApplicationType = SidekickApplicationType.Test }),
        NullLogger<DataProvider>.Instance);

    private static readonly ConcurrentDictionary<GameType, Task<List<ItemClassDefinition>>> ItemClasses = new();

    public static Task<List<ItemClassDefinition>> GetItemClasses(GameType game)
    {
        return ItemClasses.GetOrAdd(game, x => DataProvider.Read<List<ItemClassDefinition>>(x, GameDataType.ItemClasses, new GameLanguageEn()));
    }
}
