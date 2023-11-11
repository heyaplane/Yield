using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

public class PlayerPrefsSaveSystem
{
    public static string inputOverrideKey = "InputBindings";

    FileHandler fileHandler;

    public PlayerPrefsSaveSystem(FileHandler fileHandler)
    {
        this.fileHandler = fileHandler;
    }
    
    public bool SaveInputOverrides(SaveData playerPrefsData, string currentPlayerProfile)
    {
        if (string.IsNullOrEmpty(currentPlayerProfile))
        {
            Debug.LogError("Current player profile is null.");
            return false;
        }

        playerPrefsData[inputOverrideKey] = PlayerPrefs.GetString(inputOverrideKey);
        return true;
    }

    public void LoadInputOverrides(SaveData playerPrefsData)
    {
        if (!playerPrefsData.TryGetValue(inputOverrideKey, out object overrides))
        {
            playerPrefsData[inputOverrideKey] = "";
            overrides = playerPrefsData[inputOverrideKey];
        }

        SetInputOverridesToPlayerPrefs((string) overrides);
    }
    
    void SetInputOverridesToPlayerPrefs(string inputOverrides)
    {
        PlayerPrefs.DeleteAll();
        PlayerPrefs.SetString(inputOverrideKey, inputOverrides);
        PlayerPrefs.Save();
    }

    public bool TrySavePlayerPrefs(string currentPlayerProfile)
    {
        if (string.IsNullOrEmpty(currentPlayerProfile))
        {
            Debug.LogError("Current player profile is null.");
            return false;
        }

        string filePath = SaveSystemHelpers.GetPlayerPrefsFilePath(currentPlayerProfile);
        SaveData playerPrefsData = fileHandler.LoadFile(filePath);

        if (SaveInputOverrides(playerPrefsData, currentPlayerProfile))
        {
            fileHandler.TrySaveFile(filePath, playerPrefsData);
            return true;
        }
        
        return false;
    }
    
    public void LoadPlayerPrefs(string currentPlayerProfile)
    {
        string filePath = SaveSystemHelpers.GetPlayerPrefsFilePath(currentPlayerProfile);
        SaveData playerPrefsData = fileHandler.LoadFile(filePath);
        LoadInputOverrides(playerPrefsData);
    }
}
