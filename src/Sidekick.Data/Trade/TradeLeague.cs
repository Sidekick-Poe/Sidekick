using System.Text.Json.Serialization;
using Sidekick.Common.Enums;
using Sidekick.Data.Items;
namespace Sidekick.Data.Trade;

public class TradeLeague
{
    public GameType Game { get; init; }

    public string Id { get; init; } = string.Empty;

    public string Text { get; init; } = string.Empty;

    public TradeLeagueRealm Realm { get; init; }

    [JsonIgnore]
    public string Value => $"{Game.GetValueAttribute()}.{Id}";
}
