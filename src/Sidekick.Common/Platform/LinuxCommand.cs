namespace Sidekick.Common.Platform;

/// <summary>
/// Checks for command line programs on Linux.
/// </summary>
public static class LinuxCommand
{
    /// <summary>
    /// Gets a value indicating whether the command is installed in one of the folders of the PATH.
    /// </summary>
    public static bool IsAvailable(string command)
    {
        var pathEnv = Environment.GetEnvironmentVariable("PATH");
        if (string.IsNullOrEmpty(pathEnv))
        {
            return false;
        }

        foreach (var path in pathEnv.Split(':'))
        {
            if (File.Exists(Path.Combine(path, command)))
            {
                return true;
            }
        }

        return false;
    }
}
