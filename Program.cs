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

//updates the launcher
static void Update_Launcher(String installDir, string versionStr)
{
    Console.WriteLine("Launcher updating.....");

    bool VersionsMatch = KWQIWebClient.CheckVersion_GitRelease("RiskiVR", "KAR-Launcher", versionStr);
    Console.WriteLine("KAR Launcher " + (VersionsMatch ? "is already up to date" : "needs to be updated."));

    if (!VersionsMatch)
    {
        //close the launcher process if it's currently running
        bool reopen = KillProcess("KAR Launcher");

        //perform update
        KWQICommonInstalls.GetLatest_KARLauncher(new System.IO.DirectoryInfo(installDir));

        //reopen the launcher
        if (reopen)
        {
            Process launcherProcess = new Process();
            launcherProcess.StartInfo.FileName = new System.IO.DirectoryInfo(installDir).FullName + "/KAR Launcher.exe";
            launcherProcess.Start();
        }
    }

    Console.WriteLine("Launcher done updating.");
}

//updates the updater
static void Update_Updater(String installDir, string versionStr)
{
    Console.WriteLine("Updater updating.....");

    //checks for a new version
    bool VersionsMatch = KWQIWebClient.CheckVersion_GitRelease("RiskiVR", "KAR-Updater", versionStr);
    Console.WriteLine("KAR Updater " + (VersionsMatch ? "is already up to date" : "needs to be updated."));

    if (!VersionsMatch)
    {
        //close the update process if it's currently running
        bool reopen = KillProcess("KAR Updater");

        //perform update
        KWQICommonInstalls.GetLatest_KARUpdater(new System.IO.DirectoryInfo(installDir));

        //reopen the update
        if (reopen)
        {
            Process launcherProcess = new Process();
            launcherProcess.StartInfo.FileName = new System.IO.DirectoryInfo(installDir).FullName + "/KAR Updater.exe";
            launcherProcess.Start();
        }
    }

    Console.WriteLine("Updater done updating.");
}

//updates KARphin
static void UpdateKARphin(String installDir, string versionStr)
{
    Console.WriteLine("KARphin updating.....");

    //checks for a new version
    bool VersionsMatch = KWQIWebClient.CheckVersion_GitRelease("SeanMott", "KARphin_Modern", versionStr);
    Console.WriteLine("KARphin " + (VersionsMatch ? "is already up to date" : "needs to be updated."));

    if (!VersionsMatch)
    {
        //close the update process if it's currently running
        bool reopen = KillProcess("KARphin");

        //perform update
        KWQICommonInstalls.GetLatest_KARphin(KWStructure.GenerateKWStructure_Directory_NetplayClients(new System.IO.DirectoryInfo(installDir)));

        //reopen the update
        if (reopen)
        {
            Process launcherProcess = new Process();
            launcherProcess.StartInfo.FileName = new System.IO.DirectoryInfo(installDir).FullName + "/Clients/KARphin.exe";
            launcherProcess.Start();
        }
    }

    Console.WriteLine("KARphin done updating.");
}

//updates Tools
static void UpdateTools(String installDir)
{
    Console.WriteLine("Tools updating.....");

    //deletes folder
    if(Directory.Exists(installDir + "/Tools"))
        Directory.Delete(installDir + "/Tools", true);

    KWQICommonInstalls.GetLatest_Tools(KWStructure.GenerateKWStructure_Directory_Tools(new DirectoryInfo(installDir)));

    Console.WriteLine("Tools done updating.");
}

//resets the client deps
static void ResetClientDeps(String installDir)
{
    Console.WriteLine("Client Data Resetting.....");

    //close the update process if it's currently running
    KillProcess("KARphin");

    //deletes folder
    if(Directory.Exists(installDir + "/Clients"))
        Directory.Delete(installDir + "/Clients", true);

    DirectoryInfo netplay = KWStructure.GenerateKWStructure_Directory_NetplayClients(new System.IO.DirectoryInfo(installDir));

    //perform update
    KWQICommonInstalls.GetLatest_ClientDeps(netplay);
    KWQICommonInstalls.GetLatest_KARphin(netplay);

    Process launcherProcess = new Process();
    launcherProcess.StartInfo.FileName = new System.IO.DirectoryInfo(installDir).FullName + "/Clients/KARphin.exe";
    launcherProcess.Start();

    Console.WriteLine("Client Data Resetting.");
}

//defines the flags
String[] flags =
{
    "-installDir",
    "-launcher",
    "-updater",
    "-KARphin",
    "-resetClient",
    "-tools",
    "-setVer"
};

const int LAUNCHER_FLAG_INDEX = 1;
const int UPDATER_FLAG_INDEX = 2;
const int KARPHIN_FLAG_INDEX = 3;
const int RESET_CLIENT_FLAG_INDEX = 4;
const int TOOLS_FLAG_INDEX = 5;
const int INSTALL_DIR_FLAG_INDEX = 0;
const int VERSION_STRING_FLAG_INDEX = 6;

//entry point
int main(String[] args)
{
    //checks what argument was passed
    if (args.Length < 1)
    {
        Console.WriteLine("-------HELP MENU-------\n\n\n");
        //Console.WriteLine(flags[0] + " || updates the KAR Launcher\n\n");
        //Console.WriteLine(flags[1] + " || updates the KAR Updater\n\n");
        //Console.WriteLine(flags[2] + " || updates KARphin\n\n");
        //Console.WriteLine(flags[4] + " || updates the Tools directory\n\n");
        //Console.WriteLine(flags[3] + " || resets all client data\n\n");

        return -1;
    }

    //parses the arguments
    string installDir = System.Environment.CurrentDirectory, verStr = "0000000";
    bool updateLauncher = false, updateUpdater = false, updateTools = false, updateKARphin = false, resetClientData = false;

    //parses the arguments
    for (int i = 0; i < args.Length; i++)
    {
        //sets the install dir
        if (args[i] == flags[INSTALL_DIR_FLAG_INDEX])
        {
            i++;
            installDir = args[i];
            continue;
        }

        //sets the version dir
        if (args[i] == flags[VERSION_STRING_FLAG_INDEX])
        {
            i++;
            verStr = args[i];
            continue;
        }

        //launcher should update
        if (args[i] == flags[LAUNCHER_FLAG_INDEX])
        {
            updateLauncher = true;
            continue;
        }

        //updater should update
        if (args[i] == flags[UPDATER_FLAG_INDEX])
        {
            updateUpdater = true;
            continue;
        }

        //tools should update
        if (args[i] == flags[TOOLS_FLAG_INDEX])
        {
            updateTools = true;
            continue;
        }

        //karphin should update
        if (args[i] == flags[KARPHIN_FLAG_INDEX])
        {
            updateKARphin = true;
            continue;
        }

        //reset deps
        if (args[i] == flags[RESET_CLIENT_FLAG_INDEX])
        {
            resetClientData = true;
            continue;
        }
    }

    //reset client deps
    if (resetClientData)
        ResetClientDeps(installDir);

    //update karphin
    if (updateKARphin)
        UpdateKARphin(installDir, verStr);

    //update updater
    if(updateUpdater)
        Update_Updater(installDir, verStr);

    //update tools
    if (updateTools)
        UpdateTools(installDir);

    //update launcher
    if(updateLauncher)
        Update_Launcher(installDir, verStr);

    //Console.ReadKey();
    return -1;
}

//call main
main(args);