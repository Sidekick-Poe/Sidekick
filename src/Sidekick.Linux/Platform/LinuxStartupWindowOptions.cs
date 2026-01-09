namespace Sidekick.Linux.Platform;

public sealed class LinuxStartupWindowOptions(string? startupUrl)
{
    public string? StartupUrl { get; } = startupUrl;
}
