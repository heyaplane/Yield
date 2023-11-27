using UnityEngine;

public class GlobalDataManager : SingletonMonobehaviour<GlobalDataManager>
{
    public object CaptureGlobalData()
    {
        var globalData = new SaveData
        {
            {"Time", null},
            {"Files", null},
            {"Chat", null}
        };

        globalData["Time"] = TimeSystem.Instance.GetCurrentTimestamp;
        globalData["Files"] = FileSystemManager.Instance.CaptureSaveData();
        globalData["Chat"] = MessageSystemManager.Instance.CaptureSaveData();
        return globalData;
    }

    public void RestoreGlobalData(object data)
    {
        if (data is SaveData saveData)
        {
            if (saveData.TryGetValue("Files", out var fileData))
                FileSystemManager.Instance.RestoreSaveData(fileData);
            
            if (saveData.TryGetValue("Time", out var timeData))
                TimeSystem.Instance.RestoreCurrentTime(timeData);

            if (saveData.TryGetValue("Chat", out var chatData))
                MessageSystemManager.Instance.RestoreSaveData(chatData);
        }
    }
}
