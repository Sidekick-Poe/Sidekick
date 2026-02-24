namespace Sidekick.Common;

public enum SidekickApplicationType
{
    Unknown,
    Test,
    Web,
    Photino,
    Wpf,
    DataBuilder,
}

public static class SidekickApplicationTypeExtensions
{
    public static bool SupportsKeybinds(this SidekickApplicationType type) => type == SidekickApplicationType.Wpf
                                                                              || type == SidekickApplicationType.Photino;

    public static bool SupportsAuthentication(this SidekickApplicationType type) => type == SidekickApplicationType.Wpf;

    public static bool SupportsHardwareAcceleration(this SidekickApplicationType type) => type == SidekickApplicationType.Wpf;
}
