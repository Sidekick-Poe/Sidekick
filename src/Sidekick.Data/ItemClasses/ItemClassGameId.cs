namespace Sidekick.Data.ItemClasses;

[AttributeUsage(AttributeTargets.All, AllowMultiple = true)]
public class ItemClassGameId(GameType game, string id) : Attribute
{
    public GameType Game { get; } = game;
    public string Id { get; } = id;
}
