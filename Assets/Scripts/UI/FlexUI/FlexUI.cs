using System;
using UnityEditor;
using UnityEngine;

[ExecuteInEditMode]
public class FlexUI : MonoBehaviour
{
    FlexUIEventSO previousEventSO;
    [SerializeField] FlexUIEventSO currentEventSO;
    [SerializeField] ComponentFlexUIDataPair[] pairs;
    
    void OnValidate()
    {
        #if UNITY_EDITOR
        if (!Application.IsPlaying(this) && !EditorApplication.isPlayingOrWillChangePlaymode)
        {
            if (previousEventSO != null)
            {
                previousEventSO.DataSourceUpdatedEvent.RemoveListener(UpdateComponentProperties);
            }

            if (currentEventSO != null)
            {
                currentEventSO.DataSourceUpdatedEvent.AddListener(UpdateComponentProperties);
            }
        }
        previousEventSO = currentEventSO;
        #endif

        UpdateComponentProperties();
    }

    void UpdateComponentProperties()
    {
        foreach (var pair in pairs)
        {
            if (pair.DataSO == null || pair.Component == null) continue;
            pair.DataSO.UpdatePropertyValue(pair.Component);
        }
    }
}

[Serializable]
public struct ComponentFlexUIDataPair
{
    public Component Component;
    public FlexUIDataSO DataSO;
}
