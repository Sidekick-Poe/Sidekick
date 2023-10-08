// See https://aka.ms/new-console-template for more information
using Microsoft.Win32;

Console.WriteLine($"Registering SideKick:// Protocol to {args[0]}");

string customProtocol = "Sidekick";

RegistryKey? key = Registry.ClassesRoot.OpenSubKey(customProtocol);
if (key == null)
{

    key = Registry.ClassesRoot.CreateSubKey(customProtocol);
    key.SetValue(string.Empty, "URL: " + customProtocol);
    key.SetValue("URL Protocol", string.Empty);

    key = key.CreateSubKey(@"shell\open\command");
    key.SetValue(string.Empty, $"\"{args[0]}\" \"%1\"");
    key.Close();
}
