using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public abstract class BaseUniqueObject : MonoBehaviour
{
    static Dictionary<string, BaseUniqueObject> globalLookup = new();

    [DisabledInInspector] [SerializeField] string uniqueID;
    public string UniqueID => uniqueID;

    void OnValidate()
    {
        if (Application.IsPlaying(this) || gameObject.scene.name == null) return;
        UpdateGUID();
    }
    
    void UpdateGUID()
    {
#if UNITY_EDITOR
        if (string.IsNullOrEmpty(gameObject.scene.path)) return;

        var serializedObject = new SerializedObject(this);
        var property = serializedObject.FindProperty("uniqueID");

        if (string.IsNullOrEmpty(property.stringValue) || !IsUnique(property.stringValue))
        {
            property.stringValue = Guid.NewGuid().ToString();
            serializedObject.ApplyModifiedProperties();
        }

        globalLookup[property.stringValue] = this;
#endif
    }

    bool IsUnique(string candidate)
    {
        if (!globalLookup.ContainsKey(candidate)) return true;
        if (globalLookup[candidate] == this) return true;

        if (globalLookup[candidate] == null)
        {
            globalLookup.Remove(candidate);
            return true;
        }

        if (globalLookup[candidate].UniqueID != candidate)
        {
            globalLookup.Remove(candidate);
            return true;
        }

        return false;
    }
}
