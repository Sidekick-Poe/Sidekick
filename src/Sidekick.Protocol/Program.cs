using System;
using System.Diagnostics;
using System.IO;
using Microsoft.Win32;
using Sidekick.Common.Logging;

var logger = LogHelper.GetLogger("Sidekick_protocol_log.log");

void Log(string? message)
{
    if (message == null) return;
    Console.WriteLine(message);
    logger.Information(message);
}

try
{
    var customProtocol = "Sidekick";
    var key = Registry.ClassesRoot.OpenSubKey(customProtocol, true);
    if (key == null)
    {
        Log("Creating the registry key entries...");
        key = Registry.ClassesRoot.CreateSubKey(customProtocol);
    }

    key.SetValue(string.Empty, "URL: " + customProtocol);
    key.SetValue("URL Protocol", string.Empty);

    var currentDirectory = Directory.GetCurrentDirectory();
    var sidekickPath = Path.Combine(currentDirectory, "Sidekick.exe");
    Log($"Registering Protocol Sidekick:// to {sidekickPath}");

    var command = key.CreateSubKey("shell").CreateSubKey("open").CreateSubKey("command");
    command.SetValue(string.Empty, $"\"{sidekickPath}\" \"%1\"");

    command.Close();
    key.Close();

    Log("Protocol Registered!");

    Log("Sending message to Sidekick.exe...");
    var startInfo = new ProcessStartInfo(@"Sidekick.exe", "Sidekick://INSTALLED")
    {
        Verb = "runas",
        UseShellExecute = true,
    };

    Process.Start(startInfo);

    Log("Closing the application...");

}
catch (Exception e)
{
    Log(e.Message);
    Log("=============================");
    Log(e.StackTrace);
    Log("=============================");
    Log($"An exception happened while configuring the Sidekick:// protocol. If this issue persists, please contact us on Discord or Github. Press any key to close this message.");
    Console.ReadKey();
}
