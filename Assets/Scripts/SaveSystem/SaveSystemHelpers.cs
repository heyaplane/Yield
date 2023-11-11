using System;
using System.IO;
using UnityEngine;
using UnityEngine.Assertions;

public static class SaveSystemHelpers
{
    public static string GetFullFilePath(string filename) => Path.Combine(Application.persistentDataPath, filename);
    public static string GetPlayerPrefsFilePath(string currentProfileName) => GetFullFilePath(Path.Combine(currentProfileName, "PlayerPrefs.json"));
    public static string GetPlayerGamesDirectory(string currentProfileName) => GetFullFilePath(Path.Combine(currentProfileName, "saves"));
    public static string GetGameFilePath(string currentProfileName, string gameName) => GetFullFilePath(Path.Combine(GetPlayerGamesDirectory(currentProfileName), gameName + ".json"));
    
    public static bool CheckIfDirectoryAndFileExist(string filepath)
    {
        string directoryPath = Path.GetDirectoryName(filepath);
        Assert.IsNotNull(directoryPath);
        
        if (!Directory.Exists(directoryPath))
        {
            Directory.CreateDirectory(directoryPath);
        }
        
        if (!File.Exists(filepath))
        {
            return false;
        }

        return true;
    }
}
