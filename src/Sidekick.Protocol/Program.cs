using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Win32;

try
{
    var customProtocol = "Sidekick";
    var key = Registry.ClassesRoot.OpenSubKey(customProtocol, true);
    if (key == null)
    {
        Console.WriteLine("Creating the registry key entries...");
        key = Registry.ClassesRoot.CreateSubKey(customProtocol);
    }

    key.SetValue(string.Empty, "URL: " + customProtocol);
    key.SetValue("URL Protocol", string.Empty);

    var currentDirectory = Directory.GetCurrentDirectory();
    var sidekickPath = Path.Combine(currentDirectory, "Sidekick.exe");
    Console.WriteLine($"Registering Protocol Sidekick:// to {sidekickPath}");

    var command = key.CreateSubKey("shell").CreateSubKey("open").CreateSubKey("command");
    command.SetValue(string.Empty, $"\"{sidekickPath}\" \"%1\"");

    command.Close();
    key.Close();

    Console.WriteLine("Protocol Registered! Closing the application...");
    await Task.Delay(3000);
}
catch (Exception e)
{
    Console.WriteLine(e.Message);
    Console.WriteLine("=============================");
    Console.WriteLine(e.StackTrace);
    Console.WriteLine("=============================");
    Console.WriteLine($"An exception happened while configuring the sidekick:// protocol. If this issue persists, please contact us on Discord or Github. Press any key to close this message.");
    Console.ReadKey();
}
