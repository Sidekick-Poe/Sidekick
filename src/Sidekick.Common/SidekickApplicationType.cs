namespace Sidekick.Common;

public enum SidekickApplicationType
{
    Unknown,
    Test,
    Web,
    Avalonia,
}

public static class SidekickApplicationTypeExtensions
{
    public static bool SupportsKeybinds(this SidekickApplicationType type) => type == SidekickApplicationType.Avalonia;

    public static bool SupportsAuthentication(this SidekickApplicationType type) => type == SidekickApplicationType.Avalonia;
}
