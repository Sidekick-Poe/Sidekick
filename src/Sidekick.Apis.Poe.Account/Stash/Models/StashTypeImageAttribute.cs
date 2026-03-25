namespace Sidekick.Apis.Poe.Account.Stash.Models;

[AttributeUsage(AttributeTargets.All)]
public class StashTypeImageAttribute(string path) : Attribute
{
    public string Path { get; } = path;
}
