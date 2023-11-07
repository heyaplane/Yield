using System;
using UnityEngine;

public abstract class FlexUIDataSO : ScriptableObject
{
    [HideInInspector] [SerializeField] public FlexUIDataSourceSO DataSource;
    public void UpdateDataSource(FlexUIDataSourceSO newDataSource) => DataSource = newDataSource;

    public abstract void UpdatePropertyValue(Component component);
}