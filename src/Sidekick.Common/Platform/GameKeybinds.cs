namespace Sidekick.Common.Platform;

public static class GameKeybinds
{
    /// <summary>
    /// Keys that can't be used as hotkeys, since Path of Exile needs them and Sidekick sends them itself, like Ctrl+C to copy items.
    /// Ctrl+F and Ctrl+Enter are not reserved, since Sidekick replaces those game shortcuts on purpose.
    /// </summary>
    public static IReadOnlySet<string> Reserved { get; } = new HashSet<string>
    {
        "Ctrl+C",
        "Ctrl+V",
        "Ctrl+A",
        "Enter",
        "Up",
        "Left",
        "Right",
        "Home",
        "Delete",
    };
}
