using System;

public interface ISaveableComponent
{
    object CaptureSaveData();
    void RestoreSaveData(object saveData);
}
