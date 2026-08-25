using System.Text.Json.Serialization;
using Sidekick.Common.Enums;

namespace Sidekick.Game.Leagues;

public class League
{
    public GameType Game { get; init; }

    public required string Id { get; init; }

    public required string Text { get; init; }

    public string? ScoutValue { get; init; }

    public LeagueRealm Realm { get; init; }

    [JsonIgnore]
    public string Value => $"{Game.GetValueAttribute()}.{Id}";

    public override string ToString()
    {
        return Value;
    }
}
