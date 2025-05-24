namespace Sidekick.Common.Settings;

/// <summary>
/// Hotkeys to enter regex in the inventory search.
/// </summary>
public class RegexHotkey
{
    public RegexHotkey()
    {
    }

    public RegexHotkey(string key, string regex, string description)
    {
        Key = key;
        Regex = regex;
        Description = description;
    }

    public string? Key { get; set; }

    public string? Regex { get; set; }

    public string? Description { get; set; }
}
