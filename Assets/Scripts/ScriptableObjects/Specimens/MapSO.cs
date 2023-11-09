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
}

[Serializable]
public struct ChunkAssignment
{
    public int rowNum;
    public int colNum;
    public ChunkGroupSO chunkData;
}