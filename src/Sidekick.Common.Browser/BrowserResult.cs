namespace Sidekick.Common.Browser;

public class BrowserResult
{
    public bool Success { get; set; }

    public string? UserAgent { get; set; }

    public Dictionary<string, string> Cookies { get; set; } = [];

    public string? JsonContent { get; set; }

}
