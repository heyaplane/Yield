using Newtonsoft.Json;
using UnityEngine;

public class FileHandler
{
    SaveLoadToJson<SaveData> saveDataToJson;

    public FileHandler(bool useEncryption)
    {
        saveDataToJson = new SaveLoadToJson<SaveData>(new JsonFormatter<SaveData>(new SaveDataJsonConverter(), Formatting.Indented), useEncryption);
    }
    
    public bool TrySaveFile(string filePath, SaveData saveData)
    {
        string storedData = saveDataToJson.SaveIO(saveData, filePath);

        var verifiedData = LoadFile(filePath);
        if (verifiedData == null)
        {
            Debug.LogError("Save file could not be reopened and verified.");
            return false;
        }

        SaveManager.Instance.SavedData = storedData;
        return true;
    }

    public SaveData LoadFile(string filePath)
    {
        if (!SaveSystemHelpers.CheckIfDirectoryAndFileExist(filePath))
            return new SaveData();
            
        return saveDataToJson.LoadIO(filePath);
    }
}
