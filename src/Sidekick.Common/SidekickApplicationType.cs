namespace Sidekick.Common;

public enum SidekickApplicationType
{
    Unknown,
    Test,
    Web,
    Photino,
    Electron,
    Wpf,
    DataBuilder,
}

public static class SidekickApplicationTypeExtensions
{
    public static bool SupportsKeybinds(this SidekickApplicationType type) => type is SidekickApplicationType.Wpf or SidekickApplicationType.Photino or SidekickApplicationType.Electron;

    public static bool SupportsAuthentication(this SidekickApplicationType type) => type == SidekickApplicationType.Wpf;

    public static bool SupportsHardwareAcceleration(this SidekickApplicationType type) => type == SidekickApplicationType.Wpf;
}
