namespace Sidekick.Data.ItemDefinitions;

[AttributeUsage(AttributeTargets.All, AllowMultiple = true)]
public class ItemClassTradeId(GameType game, string id) : Attribute
{
    public GameType Game { get; } = game;
    public string Id { get; } = id;
}
