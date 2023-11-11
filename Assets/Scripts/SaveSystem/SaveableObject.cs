using System;
using System.Collections;
using System.Linq;
using UnityEngine;

public class SaveableObject : BaseUniqueObject
{
    void OnEnable()
    {
        if (SaveManager.Instance != null)
            SaveManager.Instance.SaveableObjects.Add(this);
    }

    void OnDestroy()
    {
        if (SaveManager.Instance != null)
            SaveManager.Instance.SaveableObjects.Remove(this);
    }

    public object CaptureSaveData()
    {
        var saveables = GetComponents<ISaveableComponent>();
        return new SaveData(saveables.ToDictionary(item => item.GetType().ToString(), item => item.CaptureSaveData()));
    }

    public void RestoreState(object saveDataObject)
    {
        if (saveDataObject is not SaveData saveData)
        {
            Debug.LogError("Could not cast object to SaveData type.");
            return;
        }

        foreach (var saveable in GetComponents<ISaveableComponent>())
        {
            string typeString = saveable.GetType().ToString();
            if (saveData.TryGetValue(typeString, out object saveDataValue))
            {
                saveable.RestoreSaveData(saveDataValue);
            }
        }
    }
}
