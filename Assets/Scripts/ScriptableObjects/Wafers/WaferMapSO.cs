using System;
using System.Collections.Generic;
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
    public SectionDataSO sectionData;
    [DisabledInInspector] [TextArea(1,2)] public string chunkLimits;
}