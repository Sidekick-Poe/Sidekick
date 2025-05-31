namespace Sidekick.Common.Browser;

public class BrowserCompletionOptions
{
    public Uri? Uri { get; set; }

    public Dictionary<string, string> Cookies { get; set; } = [];

    public string? JsonContent { get; set; }

    public bool IsJson { get; set; }
}
