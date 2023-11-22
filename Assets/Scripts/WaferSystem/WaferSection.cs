using UnityEngine;

public class WaferSection
{
    readonly int numChunksPerSectionLength;
    public Vector2Int SectionIndices { get; }
    public Vector2 NormalizedSectionCoordinate { get; }
    public string SectionLocationAsString => $"{SectionIndices.x},{SectionIndices.y}";

    public WaferSection(int numChunksPerSectionLength, Vector2Int sectionIndices, Vector2 normalizedSectionCoordinate)
    {
        this.numChunksPerSectionLength = numChunksPerSectionLength;
        SectionIndices = sectionIndices;
        NormalizedSectionCoordinate = normalizedSectionCoordinate;
    }

    public (int, int)[] GetChunkCoordinates()
    {
        (int, int)[] chunkIndices = new (int, int)[numChunksPerSectionLength * numChunksPerSectionLength];
        
        for (int k = 0; k < numChunksPerSectionLength; k++)
        {
            for (int l = 0; l < numChunksPerSectionLength; l++)
            {
                int row = SectionIndices.x * numChunksPerSectionLength + k;
                int col = SectionIndices.y * numChunksPerSectionLength + l;
                chunkIndices[k * numChunksPerSectionLength + l] = (row, col);
            }
        }

        return chunkIndices;
    }

    public bool IsChunkInsideSection(ChunkCoordinate chunkCoordinate)
    {
        return chunkCoordinate.chunkRow >= MinChunkRow && chunkCoordinate.chunkRow <= MaxChunkRow && chunkCoordinate.chunkCol >= MinChunkCol && chunkCoordinate.chunkCol <= MaxChunkCol;
    }
    
    public int MinChunkRow => SectionIndices.x * numChunksPerSectionLength;
    public int MaxChunkRow => SectionIndices.x * numChunksPerSectionLength + numChunksPerSectionLength - 1;
    public int MinChunkCol => SectionIndices.y * numChunksPerSectionLength;
    public int MaxChunkCol => SectionIndices.y * numChunksPerSectionLength + numChunksPerSectionLength - 1;
}