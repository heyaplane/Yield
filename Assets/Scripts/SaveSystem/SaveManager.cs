using System;
using System.Collections.Generic;
using UnityEngine;

public class SaveManager : SingletonMonobehaviour<SaveManager>
{
    [SerializeField] bool useEncryption;
    
    public bool ShouldSaveOnSceneChange { get; set; }
    public string CurrentPlayerProfile { get; set; }
    public string CurrentSaveGame { get; set; }
    
    public HashSet<SaveableObject> SaveableObjects { get; set; }
    
    //So I can see saved data in the inspector
    [TextArea(3,20)] [DisabledInInspector] [SerializeField] public string SavedData;

    PlayerProfileManager playerProfileManager;
    TimestampDataSaveSystem timestampDataSaveSystem;

    FileHandler fileHandler;
    PlayerPrefsSaveSystem playerPrefsSaveSystem;
    SaveGameManager saveGameManager;

    protected override void Awake()
    {
        base.Awake();
        timestampDataSaveSystem = new TimestampDataSaveSystem(useEncryption);
        playerProfileManager = new PlayerProfileManager(timestampDataSaveSystem);

        fileHandler = new FileHandler(useEncryption);
        playerPrefsSaveSystem = new PlayerPrefsSaveSystem(fileHandler);
        saveGameManager = new SaveGameManager(fileHandler);

        SaveableObjects = new HashSet<SaveableObject>();
    }

    public bool SaveTimestampData() => timestampDataSaveSystem.TrySaveTimestampData(CurrentPlayerProfile);
    public bool TrySetLatestPlayerProfile() => playerProfileManager.TrySetLatestPlayerProfile();
    public string[] GetSavedProfileNames() => playerProfileManager.GetSavedProfileNames();

    public void ChangeProfile(string profileName)
    {
        if (!string.IsNullOrEmpty(CurrentPlayerProfile))
        {
            SavePlayerPrefs();
            SaveTimestampData();
        }

        CurrentPlayerProfile = profileName;
        LoadPlayerPrefs();
        SavePlayerPrefs();
        SaveTimestampData();
        EventManager.OnRequestMainMenuTransition();
    }

    public bool SavePlayerPrefs() => playerPrefsSaveSystem.TrySavePlayerPrefs(CurrentPlayerProfile);
    public void LoadPlayerPrefs() => playerPrefsSaveSystem.LoadPlayerPrefs(CurrentPlayerProfile);

    public string[] GetSaveGameNames() => saveGameManager.GetSaveGameNames(SaveSystemHelpers.GetPlayerGamesDirectory(CurrentPlayerProfile));
    public void CreateNewGame(string gameName) => saveGameManager.CreateNewGame(CurrentPlayerProfile, gameName);
    public void SaveGameData() => saveGameManager.SaveSceneData(CurrentPlayerProfile, CurrentSaveGame);
    public void LoadGameData() => saveGameManager.LoadSceneData(CurrentPlayerProfile, CurrentSaveGame);

    public void SaveGameInitiated(string gameName)
    {
        CurrentSaveGame = gameName;
        saveGameManager.LoadInitialGameData(CurrentPlayerProfile, CurrentSaveGame);
        var nextScene = SceneController.Instance.GetSceneFromBuildIndex(saveGameManager.GetLastSavedScene());
        GameManager.Instance.RequestSceneTransition(nextScene);
    }
}
