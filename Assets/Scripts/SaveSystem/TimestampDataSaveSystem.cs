using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

public class TimestampDataSaveSystem
{
    readonly SaveLoadToJson<Dictionary<string, DateTime>> timestampToJson;
    readonly string fullTimestampFilPath;
    const string saveTimestampFilename = "save_timestamps.json";

    public TimestampDataSaveSystem(bool useEncryption)
    {
        fullTimestampFilPath = SaveSystemHelpers.GetFullFilePath(saveTimestampFilename);
        timestampToJson = new SaveLoadToJson<Dictionary<string, DateTime>>(
            new JsonFormatter<Dictionary<string, DateTime>>(null, Formatting.Indented), useEncryption);
    }
    
    public bool TrySaveTimestampData(string currentPlayerProfile)
    {
        if (string.IsNullOrEmpty(currentPlayerProfile)) return false;

        var timestampData = LoadTimestampData();

        if (timestampData == null)
        {
            Debug.LogError("Timestamp data was null.");
            return false;
        }
        
        timestampData[currentPlayerProfile] = DateTime.Now;
        timestampToJson.SaveIO(timestampData, fullTimestampFilPath);
        return true;
    }

    public Dictionary<string, DateTime> LoadTimestampData()
    {
        if (!SaveSystemHelpers.CheckIfDirectoryAndFileExist(fullTimestampFilPath))
            return new Dictionary<string, DateTime>();

        return timestampToJson.LoadIO(fullTimestampFilPath);
    }
}
