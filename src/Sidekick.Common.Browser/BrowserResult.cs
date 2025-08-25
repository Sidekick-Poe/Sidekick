namespace Sidekick.Common.Browser;

public class BrowserResult
{
    public Uri? Uri { get; init; }

    public bool Success { get; init; }

    public string? UserAgent { get; init; }

    public Dictionary<string, string> Cookies { get; init; } = [];

    public string? JsonContent { get; set; }

}
