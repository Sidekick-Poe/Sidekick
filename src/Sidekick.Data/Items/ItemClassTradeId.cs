namespace Sidekick.Data.Items;

[AttributeUsage(AttributeTargets.All)]
public class ItemClassTradeId(GameType game, string id) : Attribute
{
    public GameType Game { get; } = game;
    public string Id { get; } = id;
}
