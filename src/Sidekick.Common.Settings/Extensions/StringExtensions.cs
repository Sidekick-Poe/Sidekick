using Sidekick.Game;
namespace Sidekick.Common.Settings.Extensions;

/// <summary>
///     Class containing extension methods for strings.
/// </summary>
public static class StringExtensions
{
    public static GameType GetGameFromLeagueId(this string? leagueId)
    {
        return leagueId
                ?.Split('.')
                .ElementAtOrDefault(0) switch
            {
                "poe2" => GameType.Poe2,
                _ => GameType.Poe1,
            };
    }
}
