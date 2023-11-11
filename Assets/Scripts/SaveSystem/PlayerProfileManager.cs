using System;
using System.IO;
using System.Linq;
using UnityEngine;

public class PlayerProfileManager
{
    TimestampDataSaveSystem timestampDataSaveSystem;
    
    public PlayerProfileManager(TimestampDataSaveSystem timestampDataSaveSystem)
    {
        this.timestampDataSaveSystem = timestampDataSaveSystem;
    }
    
    public string[] GetSavedProfileNames()
    {
        var timestampData = timestampDataSaveSystem.LoadTimestampData();
        if (timestampData == null)
        {
            Debug.LogError("Timestamp data was null.");
            return null;
        }
        
        string[] directories;

        try
        {
            directories = Directory.GetDirectories(Application.persistentDataPath);
        }
        catch (Exception e)
        {
            Debug.LogError("Error: " + e);
            return null;
        }

        if (timestampData.Count == 0 || directories.Length == 0)
        {
            Debug.LogWarning("No player profiles detected.");
            return null;
        }

        return timestampData.Keys.ToArray();
    }
    
    public bool TrySetLatestPlayerProfile()
    {
        var timestampData = timestampDataSaveSystem.LoadTimestampData();
        if (timestampData == null)
        {
            Debug.LogError("Timestamp data was null.");
            return false;
        }
        
        string[] directories;

        try
        {
            directories = Directory.GetDirectories(Application.persistentDataPath);
        }
        catch (Exception e)
        {
            Debug.LogError("Error: " + e);
            return false;
        }

        if (timestampData.Count == 0 || directories.Length == 0)
        {
            Debug.LogWarning("No player profiles detected.");
            return false;
        }

        SaveManager.Instance.CurrentPlayerProfile = timestampData.OrderByDescending(kv => kv.Value).FirstOrDefault().Key;
        return true;
    }
}
