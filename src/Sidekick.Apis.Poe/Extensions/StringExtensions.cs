using Sidekick.Apis.Poe.Items;
namespace Sidekick.Apis.Poe.Extensions;

/// <summary>
///     Class containing extension methods for strings.
/// </summary>
public static class StringExtensions
{
    public static string? GetUrlSlugForLeague(this string? leagueId)
    {
        return leagueId?.Split('.').ElementAtOrDefault(1);
    }

    public static GameType GetGameFromLeagueId(this string? leagueId)
    {
        return leagueId
            ?.Split('.')
            .ElementAtOrDefault(0) switch
        {
            "poe2" => GameType.PathOfExile2,
            _ => GameType.PathOfExile,
        };
    }
}
