﻿using RedLoader.Utils;
using SonsSdk;
using SUI;
using RedLoader;
using System.Collections;
using UnityEngine.Networking;
using System.Diagnostics;
using ZombieMode.UI;
using Steamworks;

namespace ZombieMode.Core;

public class Installer
{
    public static Observable<string> TextContent = new(info);
    public static string DownloadSize;
    public static Observable<bool> IsDownloading = new(false);
    public static Observable<float> DownloadProgress = new(0);
    public static Observable<string> DownloadSpeed = new("");

    public static Observable<bool> IsInstalled = new(false);

    const string url = "https://github.com/ImAxel0/ZombieMode_SOTF/releases/download/v0.1.0/ZombieMode-Alpha1.zip";
    const string info = "This installer will download and install all of the necessary files to play ZombieMode\n" +
        "Click on the Start Download button to download the necessary files.";
    const string downloading = "The installer is now downloading the required files.\n" +
        "If it gets stuck you can use the cancel button.\n" +
        "When the progress bar reaches 100% the game will freeze for some seconds\n" +
        "while extracting the downloaded files.";

    private enum SizeUnits
    {
        Byte, KB, MB, GB, TB, PB, EB, ZB, YB
    }

    private static string ToSize(long value, SizeUnits unit)
    {
        return (value / (double)Math.Pow(1024, (long)unit)).ToString("0.00");
    }

    private static string ToSize(ulong value, SizeUnits unit)
    {
        return (value / (double)Math.Pow(1024, (long)unit)).ToString("0.00");
    }

    public static void OnDownload_ClickAsync()
    {
        IsDownloading.Set(true);
        TextContent.Set(downloading);
        DownloadFile(url, Path.Combine(LoaderEnvironment.ModsDirectory, $"{Guid.NewGuid()}.zip")).RunCoro();
    }

    public static void OnUninstall_Click()
    {
        if (Directory.Exists(Path.Combine(LoaderEnvironment.ModsDirectory, "ZombieMode")))
        {
            var files = Directory.GetFiles(Path.Combine(LoaderEnvironment.ModsDirectory, "ZombieMode"));
            foreach (var file in files)
            {
                if (file != Path.Combine(LoaderEnvironment.ModsDirectory, "ZombieMode\\manifest.json"))
                {
                    File.Delete(file);
                }
            }
        }
    }

    public static void OnCancelDownload_Click()
    {
        IsDownloading.Set(false);
        TextContent.Set(info);
        www.Dispose();
    }

    public static void OnRepair_Click()
    {
        OnUninstall_Click();
        OnDownload_ClickAsync();
    }

    public static void OnExitInstaller_Click()
    {
        InstallerUi.InstallerPanel.Active(false);
    }

    public static bool CheckInstallation()
    {
        if (Directory.Exists(Path.Combine(LoaderEnvironment.ModsDirectory, "ZombieMode")))
        {
            if (Directory.GetFiles(Path.Combine(LoaderEnvironment.ModsDirectory, "ZombieMode")).Length > 10)
            {
                return true;
            }
        }
        return false;
    }

    public static void GetDownloadSize()
    {
        GetFileSize(url, (size) =>
        {
            DownloadSize = ToSize(size, SizeUnits.MB) + " MB";
        }).RunCoro();
    }

    static UnityWebRequest www;
    private static IEnumerator DownloadFile(string url, string destPath)
    {
        www = UnityWebRequest.Get(url);
        DownloadHandler handle = www.downloadHandler;
        www.SendWebRequest();

        Stopwatch sw = Stopwatch.StartNew();

        while (!www.isDone)
        {
            DownloadProgress.Set(www.downloadProgress * 100f);
            DownloadSpeed.Value = string.Format("{0} MB/s", (www.downloadedBytes / 1024.0 / 1024.0 / sw.Elapsed.TotalSeconds).ToString("0.00"));
            yield return null;
        }

        sw.Stop();
        DownloadProgress.Set(www.downloadProgress * 100f);
        File.WriteAllBytes(destPath, www.downloadHandler.data);
        RLog.Msg($"Download finished at: {destPath}");
        IsDownloading.Set(false);
        UnZip(destPath, Path.Combine(LoaderEnvironment.ModsDirectory, "ZombieMode"));
        MoveSaveGame();
        SonsTools.ShowMessageBox("Installation completed", "Restart the game to play!");
        TextContent.Set(info);
    }

    static void MoveSaveGame()
    {
        string saveFolder = Path.Combine(Loading.GetKnownFolderPath(Loading.localLowId), $"Endnight\\SonsOfTheForest\\Saves\\{SteamUser.GetSteamID()}\\SinglePlayer\\1234567890");
        Directory.CreateDirectory(saveFolder);

        File.Move(Path.Combine(LoaderEnvironment.ModsDirectory, "ZombieMode\\SaveData.zip"),
            Path.Combine(saveFolder, "SaveData.zip"), true);
    }

    static IEnumerator GetFileSize(string url, Action<long> resut)
    {
        UnityWebRequest uwr = UnityWebRequest.Head(url);
        yield return uwr.SendWebRequest();
        string size = uwr.GetResponseHeader("Content-Length");

        if (uwr.isNetworkError || uwr.isHttpError)
        {
            RLog.Error("Error While Getting Length: " + uwr.error);
            resut?.Invoke(-1);
        }
        else
        {
            resut?.Invoke(Convert.ToInt64(size));
        }
    }

    private static void UnZip(string zipFile, string folderPath)
    {
        if (!File.Exists(zipFile))
        {
            RLog.Error($".zip file doesn't exist at: {zipFile}");
            return;
        }

        if (!Directory.Exists(folderPath))
            Directory.CreateDirectory(folderPath);

        System.IO.Compression.ZipFile.ExtractToDirectory(zipFile, folderPath);
        File.Delete(zipFile);
    }
}
