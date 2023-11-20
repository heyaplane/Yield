using System;
using System.Collections.Generic;
using UnityEngine;

public class WaferLayout
{
    int chunkDimensionSize;

    public WaferLayout(int chunkDimensionSize)
    {
        this.chunkDimensionSize = chunkDimensionSize;
    }
    
    public List<WaferSection> DivideSampleIntoSections(int gridDimensionSize, float normalizedSampleRadius)
    {
        var waferSections = new List<WaferSection>();

        if (chunkDimensionSize % gridDimensionSize != 0)
        {
            Debug.LogError("No even division of 1536x1536 chunks into the provided grid size!");
            return null;
        }
        int numChunksPerSectionLength = chunkDimensionSize / gridDimensionSize;
        float normalizedSectionSize = 1.0f / gridDimensionSize;
        
        for (int i = 0; i < gridDimensionSize; i++)
        {
            for (int j = 0; j < gridDimensionSize; j++)
            {
                float centerX = j * normalizedSectionSize + (normalizedSectionSize / 2);
                float centerY = i * normalizedSectionSize + (normalizedSectionSize / 2);
                
                Vector2 sectionCoordinates = new Vector2(centerX, centerY);
                Vector2Int sectionIndices = new Vector2Int(i, j);

                if (IsCompletelyInsideSample(normalizedSectionSize, sectionCoordinates, normalizedSampleRadius))
                {
                    waferSections.Add(new WaferSection(numChunksPerSectionLength, sectionIndices, sectionCoordinates));
                }
            }
        }

        return waferSections;
    }

    bool IsCompletelyInsideSample(float normalizedSectionSize, Vector2 center, float sampleRadius)
    {
        return Mathf.Sqrt(Mathf.Pow(0.5f - center.x - normalizedSectionSize / 2, 2) + Mathf.Pow(0.5f - center.y + normalizedSectionSize / 2, 2)) <= sampleRadius &&
               Mathf.Sqrt(Mathf.Pow(0.5f - center.x + normalizedSectionSize / 2, 2) + Mathf.Pow(0.5f - center.y + normalizedSectionSize / 2, 2)) <= sampleRadius &&
               Mathf.Sqrt(Mathf.Pow(0.5f - center.x + normalizedSectionSize / 2, 2) + Mathf.Pow(0.5f - center.y - normalizedSectionSize / 2, 2)) <= sampleRadius &&
               Mathf.Sqrt(Mathf.Pow(0.5f - center.x - normalizedSectionSize / 2, 2) + Mathf.Pow(0.5f - center.y - normalizedSectionSize / 2, 2)) <= sampleRadius;
    }
}

public class WaferSection
{
    readonly int numChunksPerSectionLength;
    public Vector2Int SectionIndices { get; }
    public Vector2 NormalizedSectionCoordinate { get; }

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
