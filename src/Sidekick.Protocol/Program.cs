// See https://aka.ms/new-console-template for more information
using Microsoft.Win32;

var currentDirectory = Directory.GetCurrentDirectory();
Console.WriteLine($"Registering SideKick:// Protocol to {currentDirectory}");

string customProtocol = "Sidekick";

RegistryKey? key = Registry.ClassesRoot.OpenSubKey(customProtocol);
if (key == null)
{
    key = Registry.ClassesRoot.CreateSubKey(customProtocol);
    key.SetValue(string.Empty, "URL: " + customProtocol);
    key.SetValue("URL Protocol", string.Empty);

    key = key.CreateSubKey(@"shell\open\command");

    // var sidekickPath = Path.Combine(currentDirectory, "Sidekick.exe");
    var sidekickPath = "C:\\Repos\\Sidekick\\src\\Sidekick.Wpf\\bin\\Debug\\net7.0-windows\\Sidekick.Wpf.exe";

    key.SetValue(string.Empty, $"\"{sidekickPath}\" \"%1\"");
    key.Close();
}
