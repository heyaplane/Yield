using System;
using UnityEngine;

[CreateAssetMenu(fileName = "MapSO", menuName = "Scriptable Object/Specimen/Map")]
public class MapSO : ScriptableObject
{
    [SerializeField] int numRows;
    public int NumRows => numRows;

    [SerializeField] int numCols;
    public int NumCols => numCols;

    [SerializeField] ChunkAssignment[] chunkGroupAssignments;
    public ChunkAssignment[] ChunkGroupAssignments => chunkGroupAssignments;

    [SerializeField] ChunkGroupSO[] randomChunkPool;
    public ChunkGroupSO[] RandomChunkPool => randomChunkPool;

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