namespace Sidekick.Common.Platform;

/// <summary>
/// Platform names for [SupportedOSPlatform] attributes, so the analyzer (CA1416) reports calls to platform-only code that
/// are not guarded by OperatingSystem.IsWindows() or OperatingSystem.IsLinux().
/// </summary>
public static class PlatformNames
{
    public const string Windows = "windows";

    public const string Linux = "linux";
}
