using System;
using System.IO;
using Microsoft.Win32;

var customProtocol = "Sidekick";
var key = Registry.ClassesRoot.OpenSubKey(customProtocol);
if (key != null)
{
    Console.WriteLine("Registry key is already installed. Closing the application...");
    return;
}

key = Registry.ClassesRoot.CreateSubKey(customProtocol);
key.SetValue(string.Empty, "URL: " + customProtocol);
key.SetValue("URL Protocol", string.Empty);

var currentDirectory = Directory.GetCurrentDirectory();
var sidekickPath = Path.Combine(currentDirectory, "Sidekick.exe");
Console.WriteLine($"Registering Protocol Sidekick:// to {sidekickPath}");

var command = key.CreateSubKey("shell").CreateSubKey("open").CreateSubKey("command");
command.SetValue(string.Empty, $"\"{sidekickPath}\" \"%1\"");

command.Close();
key.Close();

Console.WriteLine($"Protocol Registered! Closing the application...");
