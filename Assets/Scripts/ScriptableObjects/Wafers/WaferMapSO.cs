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

    [SerializeField] ChunkGroupSO passingChunkGroup;
    [SerializeField] ChunkGroupSO failingChunkGroup;
    [SerializeField] ErrorType errorType;
    public ChunkAssignment[] ChunkGroupAssignments => GetChunkGroupAssignments();

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

    public ChunkAssignment[] GetChunkGroupAssignments()
    {
        var chunkAssignments = new List<ChunkAssignment>();
        for (int i = 34; i < 223; i += 2)
        {
            for (int j = 0; j < 18; j++)
            {
                if ((i < 64 && j < 4) || (i > 192 && j < 4) || (i < 64 && j > 14) || (i > 192 && j > 14)) continue;
                switch (errorType)
                {
                    case ErrorType.Passing:
                        chunkAssignments.Add(new ChunkAssignment
                        {
                            rowNum = i + j,
                            colNum = (j * 10) + 36,
                            chunkData = passingChunkGroup
                        });
                        break;
                    case ErrorType.RadialFailOutside:
                        if (i < 96 || i > 160 || j < 6 || j > 12)
                        {
                            chunkAssignments.Add(new ChunkAssignment
                            {
                                rowNum = i + j,
                                colNum = (j * 10) + 36,
                                chunkData = failingChunkGroup
                            });
                        }
                        else
                        {
                            chunkAssignments.Add(new ChunkAssignment
                            {
                                rowNum = i + j,
                                colNum = (j * 10) + 36,
                                chunkData = passingChunkGroup
                            });
                        }
                        break;
                    case ErrorType.RadialFailInside:
                        if (i > 96 && i < 160 && j > 6 && j < 12)
                        {
                            chunkAssignments.Add(new ChunkAssignment
                            {
                                rowNum = i + j,
                                colNum = (j * 10) + 36,
                                chunkData = failingChunkGroup
                            });
                        }
                        else
                        {
                            chunkAssignments.Add(new ChunkAssignment
                            {
                                rowNum = i + j,
                                colNum = (j * 10) + 36,
                                chunkData = passingChunkGroup
                            });
                        }
                        break;
                    case ErrorType.UniformFail:
                        chunkAssignments.Add(new ChunkAssignment
                        {
                            rowNum = i + j,
                            colNum = (j * 10) + 36,
                            chunkData = failingChunkGroup
                        });
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        return chunkAssignments.ToArray();
    }
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