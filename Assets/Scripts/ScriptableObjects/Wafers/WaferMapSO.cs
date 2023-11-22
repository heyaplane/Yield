using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "MapSO", menuName = "Scriptable Object/Wafer/Wafer Map")]
public class WaferMapSO : ScriptableObject
{
    [Header("Section Data")] 
    [SerializeField] int sectionDimSize;
    public int SectionDimSize => sectionDimSize;

    [SerializeField] List<SectionAssignment> sectionAssignments;
    public List<SectionAssignment> SectionAssignments => sectionAssignments;
    public void SetSectionAssignments(List<SectionAssignment> sectionAssignments) => this.sectionAssignments = sectionAssignments;

    public List<SectionData> GetSectionDataFromLocation(Vector2Int sectionLocation) =>
        sectionAssignments.FirstOrDefault(x => sectionLocation == new Vector2Int(x.rowNum, x.colNum)).sectionData;

    [SerializeField] List<DataFeature> waferFeatures;
    public List<DataFeature> WaferFeatures => waferFeatures;
    
    [Header("Chunk Data")]
    [SerializeField] int chunkDimSize;
    public int ChunkDimSize => chunkDimSize;

    [SerializeField] ChunkAssignment[] chunkGroupAssignments;
    public ChunkAssignment[] ChunkGroupAssignments => chunkGroupAssignments;

    [SerializeField] ChunkGroupSO[] randomChunkPool;
    public ChunkGroupSO[] RandomChunkPool => randomChunkPool;

    [Header("Sprite Data")]
    [SerializeField] int lowResSpriteSize;
    public int LowResSpriteSize => lowResSpriteSize;

    [SerializeField] int medResSpriteSize;
    public int MedResSpriteSize => medResSpriteSize;

    [SerializeField] int highResSpriteSize;
    public int HighResSpriteSize => highResSpriteSize;

    [SerializeField] int pixelsPerUnit;
    public int PixelsPerUnit => pixelsPerUnit;

    public int GetGroupSpriteSize(ChunkResolution res)
    {
        return res switch
        {
            ChunkResolution.Low => lowResSpriteSize,
            ChunkResolution.Med => medResSpriteSize,
            ChunkResolution.High => highResSpriteSize,
            _ => throw new ArgumentOutOfRangeException(nameof(res), res, null)
        };
    }

    public float GetCurrentScaleFactor(ChunkResolution chunkResolution) => 0.008f * (8192.0f / GetGroupSpriteSize(chunkResolution) * pixelsPerUnit);
}

[Serializable]
public struct ChunkAssignment
{
    public int rowNum;
    public int colNum;
    public ChunkGroupSO chunkData;
}

[Serializable]
public struct SectionAssignment
{
    public int rowNum;
    public int colNum;
    public List<SectionData> sectionData;
    [DisabledInInspector] [TextArea(1,2)] public string chunkLimits;
}

[Serializable]
public struct SectionData
{
    public DataFeature Feature;
    public float Mean;
    public float StDev;
}

[Serializable]
public struct DataFeature
{
    public string FeatureName;
    public string Units;
}