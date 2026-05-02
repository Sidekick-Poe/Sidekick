namespace Sidekick.Common;

public enum SidekickApplicationType
{
    Unknown,
    Test,
    Web,
    Photino,
    Electron,
    Wpf,
    Maui,
    Avalonia,
    DataBuilder,
}

public static class SidekickApplicationTypeExtensions
{
    public static bool SupportsKeybinds(this SidekickApplicationType type) => type is SidekickApplicationType.Wpf or SidekickApplicationType.Photino or SidekickApplicationType.Electron or SidekickApplicationType.Maui or SidekickApplicationType.Avalonia;

    public static bool SupportsAuthentication(this SidekickApplicationType type) => type == SidekickApplicationType.Wpf || type == SidekickApplicationType.Maui || type == SidekickApplicationType.Avalonia;

    public static bool SupportsHardwareAcceleration(this SidekickApplicationType type) => type == SidekickApplicationType.Wpf || type == SidekickApplicationType.Maui || type == SidekickApplicationType.Avalonia;
}
