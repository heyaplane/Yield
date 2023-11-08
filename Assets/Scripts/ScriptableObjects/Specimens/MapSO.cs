using System;
using UnityEngine;

[CreateAssetMenu(fileName = "MapSO", menuName = "Scriptable Object/Specimen/Map")]
public class MapSO : ScriptableObject
{
    [SerializeField] int numRows;
    public int NumRows => numRows;

    [SerializeField] int numCols;
    public int NumCols => numCols;

    [SerializeField] int chunkDim;
    public int ChunkDim => chunkDim;
    
    [SerializeField] ChunkAssignment[] chunkCoordinates;
    public ChunkAssignment[] ChunkCoordinates => chunkCoordinates;

    [SerializeField] ChunkSO[] randomChunkPool;
    public ChunkSO[] RandomChunkPool => randomChunkPool;

    [SerializeField] int spriteSize;
    public int SpriteSize => spriteSize;

    [SerializeField] int pixelsPerUnit;
    public int PixelsPerUnit => pixelsPerUnit;

    public int CoordinateWorldSize => spriteSize / pixelsPerUnit;
}

[Serializable]
public struct ChunkAssignment
{
    public int rowNum;
    public int colNum;
    public ChunkSO chunkData;
}