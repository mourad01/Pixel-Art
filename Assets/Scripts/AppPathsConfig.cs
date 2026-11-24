using System;
using System.IO;
using System.Text;
using UnityEngine;

public static class AppPathsConfig
{
    public static string TermsUrl = "http://inotechvn.com/pixelart/terms.html";
    public static string PrivacyUrl = "http://inotechvn.com/pixelart/policy.html";

    public static string SavesPath { get; private set; }

    public static string PhotosPath { get; private set; }

    public static string SavedImagesPath { get; private set; }

    public static string DownloadsPath { get; private set; }

    public static string StreamingAssetsPath { get; private set; }

    public static string ImagesFile { get; private set; }   

    public static string PhotoFileList { get; private set; }

    public static string Host { get; private set; } 

    public static void Init()
    {
        Host = "https://inotechvn.com/pixelart/";
        SavesPath = Application.persistentDataPath + "/Worked/";
        PhotosPath = Application.persistentDataPath + "/MyArt/";
        SavedImagesPath = Application.persistentDataPath + "/Images/";
        DownloadsPath = Application.persistentDataPath + "/Downloaded/";
        StreamingAssetsPath = Application.streamingAssetsPath + "/";
        ImagesFile = "images.xml"; 
        PhotoFileList = "myphotos.bin";
        if (!Directory.Exists(SavesPath))
        {
            Directory.CreateDirectory(SavesPath);
        }
        if (!Directory.Exists(PhotosPath))
        {
            Directory.CreateDirectory(PhotosPath);
        }
        if (!Directory.Exists(SavedImagesPath))
        {
            Directory.CreateDirectory(SavedImagesPath);
        }
        if (!Directory.Exists(DownloadsPath))
        {
            Directory.CreateDirectory(DownloadsPath);
        }
        ClearCache();
    }

    public static void SetHost(string host)
    {
        Host = host; 
    }

    static void ClearCache()
    {
        try
        {
            var stringBuilder = new StringBuilder();
            var files = Directory.GetFiles(Application.temporaryCachePath);
            var cacheFiles = files;
            foreach (string text in cacheFiles)
            {
                stringBuilder.Append(text).Append("\n");
                File.Delete(text);
            }
            files = Directory.GetFiles(Application.temporaryCachePath);
        }
        catch (Exception ex)
        {
            Debug.LogError("ClearCacheError: " + ex.Message);
        }
    }
}


