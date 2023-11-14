using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public class SaveGameManager
{
    const string gameNameKey = "GameName";
    const string globalGameDataKey = "GlobalGameData";
    const string sceneAtLastSaveKey = "SceneAtLastSave";

    FileHandler fileHandler;
    SaveData currentSaveData;

    public int GetLastSavedScene() => (int) currentSaveData[sceneAtLastSaveKey];

    public SaveGameManager(FileHandler fileHandler)
    {
        this.fileHandler = fileHandler;
    }

    public string[] GetSaveGameNames(string directoryPath)
    {
        if (!Directory.Exists(directoryPath))
            Directory.CreateDirectory(directoryPath);
        
        var filenames = Directory.GetFiles(directoryPath);
        return filenames.Select(x => Path.GetFileNameWithoutExtension(x).ToString()).ToArray();
    }

    public void CreateNewGame(string currentPlayerProfile, string gameName)
    {
        string filePath = SaveSystemHelpers.GetGameFilePath(currentPlayerProfile, gameName);
        var newGameData = new SaveData();
        newGameData[sceneAtLastSaveKey] = SceneController.Instance.NewGameScene.SceneIndex;
        newGameData[gameNameKey] = gameName;
        newGameData[globalGameDataKey] = new SaveData();
        fileHandler.TrySaveFile(filePath, newGameData);
    }

    public void SaveSceneData(string currentPlayerProfile, string gameName)
    {
        if (!SaveManager.Instance.ShouldSaveOnSceneChange) return;
        
        string filePath = SaveSystemHelpers.GetGameFilePath(currentPlayerProfile, gameName);
        if (currentSaveData == null || (string) currentSaveData[gameNameKey] != gameName)
            currentSaveData = fileHandler.LoadFile(filePath);

        CaptureGameData(currentSaveData);
        if(!fileHandler.TrySaveFile(filePath, currentSaveData))
            Debug.LogError("Unable to save game.");
    }

    public void LoadInitialGameData(string currentPlayerProfile, string gameName)
    {
        string filePath = SaveSystemHelpers.GetGameFilePath(currentPlayerProfile, gameName);
        currentSaveData = fileHandler.LoadFile(filePath);
    }

    public void LoadSceneData(string currentPlayerProfile, string gameName)
    {
        LoadInitialGameData(currentPlayerProfile, gameName);
        RestoreGameData(currentSaveData);
    }

    void CaptureGameData(SaveData saveData)
    {
        var currentSceneSO = SceneController.Instance.GetActiveSceneSO();
        string currentSceneIndex = currentSceneSO.SceneIndex.ToString();
        
        if (!saveData.TryGetValue(currentSceneIndex, out var sceneDataObj))
        {
            sceneDataObj = new SaveData();
            saveData[currentSceneIndex] = sceneDataObj;
        }

        if (sceneDataObj is SaveData sceneData)
        {
            foreach (var saveable in SaveManager.Instance.SaveableObjects)
            {
                sceneData[saveable.UniqueID] = saveable.CaptureSaveData();
            }
        }

        saveData[globalGameDataKey] = GlobalDataManager.Instance.CaptureGlobalData();
        saveData[sceneAtLastSaveKey] = currentSceneSO.SceneIndex;
    }

    void RestoreGameData(SaveData saveData)
    {
        var currentSceneSO = SceneController.Instance.GetActiveSceneSO();
        string currentSceneIndex = currentSceneSO.SceneIndex.ToString();
        
        if (saveData.TryGetValue(currentSceneIndex, out var sceneDataObj))
        {
            if (sceneDataObj is SaveData sceneData)
            {
                foreach (var saveable in SaveManager.Instance.SaveableObjects)
                {
                    string id = saveable.UniqueID;
                    if (sceneData.ContainsKey(saveable.UniqueID))
                    {
                        saveable.RestoreState(sceneData[id]);
                    }
                }
            }
        }

        if (saveData.TryGetValue(globalGameDataKey, out var globalData))
            GlobalDataManager.Instance.RestoreGlobalData(globalData);
    }
}
