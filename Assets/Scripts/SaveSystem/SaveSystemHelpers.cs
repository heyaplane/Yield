using System;
using System.IO;
using Newtonsoft.Json.Linq;
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

    public static object UnpackJObject(JObject itemObject)
    {
        string typeString = itemObject["Type"]?.ToObject<string>();
        if (typeString == null) return null;
        
        Type valueType = Type.GetType(typeString);
        JToken valueToken = itemObject["Value"];
        if (valueToken == null || valueType == null) return null;
        
        return valueToken.ToObject(valueType);
    }
}
