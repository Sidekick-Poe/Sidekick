namespace Sidekick.Common.Folder;

public interface IFolderProvider
{
    /// <summary>
    /// Opens the folder where the data files are stored.
    /// 
    /// <para>Windows: C:\Users\___\AppData\Roaming</para>
    /// <para>Linux: /home/___/.config</para>
    /// <para>OSX: /Users/___/.config</para>
    /// </summary>
    void OpenDataFolderPath();

    /// <summary>
    /// Gets the folder path where the data files are stored.
    /// </summary>
    public string GetDataFolderPath();
}
