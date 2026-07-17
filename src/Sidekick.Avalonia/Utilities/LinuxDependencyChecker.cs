using System.Diagnostics;
using System.Text;
namespace Sidekick.Avalonia.Utilities;

public static class LinuxDependencyChecker
{
    private const int ProcessTimeoutMilliseconds = 3000;

    public static async Task<IReadOnlyList<MissingLinuxDependency>> GetMissingDependencies()
    {
        if (!OperatingSystem.IsLinux())
        {
            return [];
        }

        var missingDependencies = new List<MissingLinuxDependency>();

        if (!await CommandExists("xsel"))
        {
            missingDependencies.Add(new MissingLinuxDependency("xsel", "Required for clipboard support on Linux."));
        }

        if (!await HasWpeWebKit())
        {
            missingDependencies.Add(new MissingLinuxDependency("WPE WebKit runtime", "Required for displaying Sidekick's interface on Linux."));
        }

        return missingDependencies;
    }

    public static string BuildDialogMessage(IReadOnlyList<MissingLinuxDependency> missingDependencies)
    {
        var builder = new StringBuilder();

        builder.AppendLine("Missing Linux dependencies");
        builder.AppendLine();
        builder.AppendLine("Sidekick requires the following packages on Linux:");
        builder.AppendLine();

        foreach (var dependency in missingDependencies)
        {
            builder.AppendLine($"- {dependency.Name}: {dependency.Description}");
        }

        builder.AppendLine();
        builder.AppendLine("Install them using one of the following commands:");
        builder.AppendLine();
        builder.AppendLine("Arch-based:");
        builder.AppendLine("sudo pacman -S --needed xsel wpewebkit");
        builder.AppendLine();
        builder.AppendLine("Ubuntu / Debian:");
        builder.AppendLine("sudo apt install xsel libwpewebkit-2.0-1");
        builder.AppendLine();
        builder.AppendLine("Fedora:");
        builder.AppendLine("sudo dnf install xsel wpewebkit");
        builder.AppendLine();
        builder.AppendLine("Sidekick will now close.");

        return builder.ToString();
    }

    private static async Task<bool> CommandExists(string command)
    {
        var result = await RunProcess("sh", $"-c \"command -v {EscapeShellArgument(command)}\"");
        return result.ExitCode == 0 && !string.IsNullOrWhiteSpace(result.StandardOutput);
    }

    private static async Task<bool> HasWpeWebKit()
    {
        if (await PkgConfigPackageExists("wpe-webkit-2.0"))
        {
            return true;
        }

        var ldconfigResult = await RunProcess("sh", "-c \"ldconfig -p 2>/dev/null | grep -E 'libWPEWebKit-2\\.0\\.so|libwpe-1\\.0\\.so'\"");
        if (ldconfigResult.ExitCode == 0 && !string.IsNullOrWhiteSpace(ldconfigResult.StandardOutput))
        {
            return true;
        }

        return File.Exists("/usr/lib/libWPEWebKit-2.0.so.1")
               || File.Exists("/usr/lib64/libWPEWebKit-2.0.so.1")
               || File.Exists("/usr/lib/x86_64-linux-gnu/libWPEWebKit-2.0.so.1")
               || Directory.Exists("/usr/lib/wpe-webkit-2.0")
               || Directory.Exists("/usr/lib64/wpe-webkit-2.0");
    }

    private static async Task<bool> PkgConfigPackageExists(string packageName)
    {
        if (!await CommandExists("pkg-config"))
        {
            return false;
        }

        var result = await RunProcess("pkg-config", $"--exists {packageName}");
        return result.ExitCode == 0;
    }

    private static async Task<ProcessResult> RunProcess(string fileName, string arguments)
    {
        try
        {
            using var process = new Process();
            process.StartInfo = new ProcessStartInfo
            {
                FileName = fileName,
                Arguments = arguments,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true,
            };

            process.Start();

            var standardOutputTask = process.StandardOutput.ReadToEndAsync();
            var standardErrorTask = process.StandardError.ReadToEndAsync();

            var completedTask = await Task.WhenAny(
                process.WaitForExitAsync(),
                Task.Delay(ProcessTimeoutMilliseconds));

            if (completedTask is not { } waitForExitTask || waitForExitTask != process.WaitForExitAsync())
            {
                TryKill(process);
                return new ProcessResult(-1, string.Empty, "Process timed out.");
            }

            return new ProcessResult(
                process.ExitCode,
                await standardOutputTask,
                await standardErrorTask);
        }
        catch (Exception ex)
        {
            return new ProcessResult(-1, string.Empty, ex.Message);
        }
    }

    private static string EscapeShellArgument(string value)
    {
        return value.Replace("'", "'\"'\"'");
    }

    private static void TryKill(Process process)
    {
        try
        {
            if (!process.HasExited)
            {
                process.Kill(true);
            }
        }
        catch
        {
            // Ignore cleanup errors.
        }
    }
}

public sealed record MissingLinuxDependency(string Name, string Description);

public sealed record ProcessResult(int ExitCode, string StandardOutput, string StandardError);