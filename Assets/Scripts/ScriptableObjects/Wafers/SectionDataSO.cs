using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SectionDataSO", menuName = "Scriptable Object/Wafer/Section Data")]
public class SectionDataSO : ScriptableObject
{
    [SerializeField] List<SectionData> sectionData;
    public List<SectionData> SectionData => sectionData;
}

[Serializable]
public struct SectionData
{
    public string Name;
    public string Units;
    public float Mean;
    public float StDev;
}