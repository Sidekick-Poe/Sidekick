namespace Sidekick.Common.Platform.Linux.Dependencies;

/// <summary>
/// The command that installs the missing packages on a family of distributions.
/// </summary>
public record LinuxInstallCommand(string Distribution, string Command);
