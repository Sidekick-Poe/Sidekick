using System.Diagnostics;
using Microsoft.Extensions.Logging;

namespace Sidekick.Common.Folder;

public class FolderProvider(ILogger<FolderProvider> logger) : IFolderProvider
{
    public void OpenDataFolderPath()
    {
        try
        {
            using var process = new Process();
            process.StartInfo = new ProcessStartInfo
            {
                FileName = SidekickPaths.GetDataFilePath(),
                UseShellExecute = true,
                Verb = "open"
            };
            process.Start();
        }
        catch
        {
            logger.LogError("[Folder] Failed to open data file path.");
        }
    }

    public string GetDataFolderPath()
    {
        return SidekickPaths.GetDataFilePath();
    }
}
