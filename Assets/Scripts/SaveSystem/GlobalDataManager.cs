using UnityEngine;

public class GlobalDataManager : SingletonMonobehaviour<GlobalDataManager>
{
    public object CaptureGlobalData()
    {
        var filesaveData = FileSystemManager.Instance.CaptureSaveData();
        return filesaveData;
    }

    public void RestoreGlobalData(object saveData)
    {
        FileSystemManager.Instance.RestoreSaveData(saveData);
    }
}
