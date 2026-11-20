using System.Diagnostics;
using Microsoft.Extensions.Logging;

namespace Sidekick.Common.Folder;

public class FolderProvider(ILogger<FolderProvider> logger) : IFolderProvider
{
    public void OpenDataFolderPath()
    {
        OpenFolder(SidekickPaths.GetDataFilePath());
    }

    public void OpenFolder(string path)
    {
        try
        {
            using var process = new Process();
            process.StartInfo = new ProcessStartInfo
            {
                FileName = path,
                UseShellExecute = true,
                Verb = "open"
            };
            process.Start();
        }
        catch (Exception e)
        {
            logger.LogError(e, "[Folder] Failed to open the folder {Path}.", path);
        }
    }
}
