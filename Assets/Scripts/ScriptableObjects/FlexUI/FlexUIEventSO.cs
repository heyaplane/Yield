using System;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "FlexUIEventSO", menuName = "Scriptable Object/FlexUI/FlexUI Data Updated Event")]
public class FlexUIEventSO : ScriptableObject
{
    [SerializeField] public UnityEvent DataSourceUpdatedEvent; 

    void OnEnable()
    {
        if (DataSourceUpdatedEvent == null)
        {
            DataSourceUpdatedEvent = new UnityEvent();
        }
    }
}