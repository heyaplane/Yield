using UnityEngine;

public class GlobalDataManager : SingletonMonobehaviour<GlobalDataManager>
{
    public object CaptureGlobalData()
    {
        return new SaveData();
    }

    public void RestoreGlobalData(object o)
    {
    }
}
