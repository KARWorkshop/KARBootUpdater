using System;
using System.Diagnostics;
using static System.Net.Mime.MediaTypeNames;
using System.Net;
using System.Text;
using System.Runtime.CompilerServices;

//kills the process
static bool KillProcess(string processName)
{
    // Get all processes with the given name
    Process[] processes = Process.GetProcessesByName(processName);
    bool processNeedsToBeReOpened = false;

    foreach (var process in processes)
    {
        processNeedsToBeReOpened = true;
        try
        {
            // Try to close the process gracefully
            process.CloseMainWindow();
            process.WaitForExit(5000);  // Wait 5 seconds for the process to exit

            if (!process.HasExited)
            {
                // If it hasn't exited, kill it forcefully
                process.Kill();
            }

            Console.WriteLine($"{processName} closed successfully.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error closing {processName}: {ex.Message}");
        }
    }

    return processNeedsToBeReOpened;
}

//the flags and what they do

//entry point
int main(String[] args)
{
    //checks what argument was passed
    if(args.Length < 1)
    {
        Console.WriteLine("This program is not meant to be ran by a user. It's only for KAR Launcher and KAR Updater.");
        Console.ReadKey();
        return -1;
    }

    //if launcher should update
    if (args[0] == "-launcher")
    {
        Console.WriteLine("Launcher updating.....");

        bool VersionsMatch = KWQIWebClient.CheckVersion_GitRelease("RiskiVR", "KAR-Launcher", args[1]);
        Console.WriteLine("KAR Launcher " + (VersionsMatch ? "is already up to date" : "needs to be updated."));

        if (!VersionsMatch)
        {
            //close the launcher process if it's currently running
            bool reopen = KillProcess("KAR Launcher");

            //perform update
            KWQICommonInstalls.GetLatest_KARLauncher(new System.IO.DirectoryInfo(System.Environment.CurrentDirectory));

            //reopen the launcher
            if (reopen)
            {
                Process launcherProcess = new Process();
                launcherProcess.StartInfo.FileName = new System.IO.DirectoryInfo(System.Environment.CurrentDirectory).FullName + "/KAR Launcher.exe";
                launcherProcess.Start();
            }
        }

        Console.WriteLine("Launcher done updating.");

        return 0;
    }

    //if updater should update
    else if (args[0] == "-updater")
    {
        Console.WriteLine("Updater updating.....");

        //checks for a new version
        bool VersionsMatch = KWQIWebClient.CheckVersion_GitRelease("RiskiVR", "KAR-Updater", args[1]);
        Console.WriteLine("KAR Updater " + (VersionsMatch ? "is already up to date" : "needs to be updated."));

        if (!VersionsMatch)
        {
            //close the update process if it's currently running
            bool reopen = KillProcess("KAR Updater");

            //perform update
            KWQICommonInstalls.GetLatest_KARUpdater(new System.IO.DirectoryInfo(System.Environment.CurrentDirectory));

            //reopen the update
            if (reopen)
            {
                Process launcherProcess = new Process();
                launcherProcess.StartInfo.FileName = new System.IO.DirectoryInfo(System.Environment.CurrentDirectory).FullName + "/KAR Updater.exe";
                launcherProcess.Start();
            }
        }

        Console.WriteLine("Updater done updating.");
        return 0;
    }

    //gets the latest KARphin
    else if (args[0] == "-KARphin")
    {
        Console.WriteLine("KARphin updating.....");

        //checks for a new version
        bool VersionsMatch = KWQIWebClient.CheckVersion_GitRelease("SeanMott", "KARphin_Modern", args[1]);
        Console.WriteLine("KARphin " + (VersionsMatch ? "is already up to date" : "needs to be updated."));

        if (!VersionsMatch)
        {
            //close the update process if it's currently running
            bool reopen = KillProcess("KARphin");

            //perform update
            KWQICommonInstalls.GetLatest_KARphin(KWStructure.GenerateKWStructure_Directory_NetplayClients(new System.IO.DirectoryInfo(System.Environment.CurrentDirectory)));

            //reopen the update
            if (reopen)
            {
                Process launcherProcess = new Process();
                launcherProcess.StartInfo.FileName = new System.IO.DirectoryInfo(System.Environment.CurrentDirectory).FullName + "/KARphin.exe";
                launcherProcess.Start();
            }
        }

        Console.WriteLine("KARphin done updating.");
        return 0;
    }

    //gets the latest client deps
    else if (args[0] == "-resetClient")
    {
        Console.WriteLine("Client Data Resetting.....");

        //close the update process if it's currently running
        bool reopen = KillProcess("KARphin");

        //deletes folder
        if(File.Exists(System.Environment.CurrentDirectory + "/Clients"))
            Directory.Delete(System.Environment.CurrentDirectory + "/Clients", true);

        DirectoryInfo netplay = KWStructure.GenerateKWStructure_Directory_NetplayClients(new System.IO.DirectoryInfo(System.Environment.CurrentDirectory));

        //perform update
        KWQICommonInstalls.GetLatest_ClientDeps(netplay);
        KWQICommonInstalls.GetLatest_KARphin(netplay);

        //reopen the update
        if (reopen)
        {
            Process launcherProcess = new Process();
            launcherProcess.StartInfo.FileName = new System.IO.DirectoryInfo(System.Environment.CurrentDirectory).FullName + "/KARphin.exe";
            launcherProcess.Start();
        }

        Console.WriteLine("Client Data Resetting.");
        return 0;
    }

    //gets the latest tools
    else if (args[0] == "-tools")
    {
        Console.WriteLine("Tools updating.....");

        //deletes folder
        if (File.Exists(System.Environment.CurrentDirectory + "/Tools"))
            Directory.Delete(System.Environment.CurrentDirectory + "/Tools", true);

        KWQICommonInstalls.GetLatest_Tools(KWStructure.GenerateKWStructure_Directory_Tools(new DirectoryInfo(System.Environment.CurrentDirectory)));

        Console.WriteLine("Tools done updating.");
        return 0;
    }

    Console.ReadKey();
    return -1;
}

//call main
main(args);