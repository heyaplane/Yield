using System;
using UnityEngine;

[CreateAssetMenu(fileName = "ChunkGroupSO", menuName = "Scriptable Object/Specimen/Chunk Group")]
public class ChunkGroupSO : ScriptableObject
{
    [SerializeField] ChunkSO lowResChunk;
    public ChunkSO LowResChunk => lowResChunk;

    [SerializeField] ChunkSO medResChunk;
    public ChunkSO MedResChunk => medResChunk;

    [SerializeField] ChunkSO highResChunk;
    public ChunkSO HighResChunk => highResChunk;

    public ChunkSO GetChunkFromResolution(ChunkResolution res)
    {
        return res switch
        {
            ChunkResolution.Low => lowResChunk,
            ChunkResolution.Med => medResChunk,
            ChunkResolution.High => highResChunk,
            _ => throw new ArgumentOutOfRangeException(nameof(res), res, null)
        };
    }
}

public enum ChunkResolution
{
    Low, Med, High
}