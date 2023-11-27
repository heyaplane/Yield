using System;
using System.Collections.Generic;
using UnityEngine;

public class WaferLayout
{
    int chunkDimensionSize, gridDimensionSize, numChunksPerSectionLength;
    float normalizedSectionSize;
    
    public List<WaferSection> WaferSections { get; private set; }

    public WaferLayout(int chunkDimensionSize, int gridDimensionSize)
    {
        this.chunkDimensionSize = chunkDimensionSize;
        this.gridDimensionSize = gridDimensionSize;
        numChunksPerSectionLength = chunkDimensionSize / gridDimensionSize;
        normalizedSectionSize = 1.0f / gridDimensionSize;
        
        if (chunkDimensionSize % gridDimensionSize != 0)
        {
            Debug.LogError("No even division of 1536x1536 chunks into the provided grid size!");
        }
    }
    
    public List<WaferSection> DivideWaferIntoSections(float normalizedSampleRadius)
    {
        var waferSections = new List<WaferSection>();
        
        for (int i = 0; i < gridDimensionSize; i++)
        {
            for (int j = 0; j < gridDimensionSize; j++)
            {
                float centerX = j * normalizedSectionSize + (normalizedSectionSize / 2);
                float centerY = i * normalizedSectionSize + (normalizedSectionSize / 2);
                
                Vector2 sectionCoordinates = new Vector2(centerX, centerY);
                Vector2Int sectionIndices = new Vector2Int(i, j);

                if (IsCompletelyInsideSample(sectionCoordinates, normalizedSampleRadius))
                {
                    waferSections.Add(new WaferSection(numChunksPerSectionLength, sectionIndices, sectionCoordinates));
                }
            }
        }

        WaferSections = waferSections;
        return waferSections;
    }

    bool IsCompletelyInsideSample(Vector2 center, float sampleRadius)
    {
        return Mathf.Sqrt(Mathf.Pow(0.5f - center.x - normalizedSectionSize / 2, 2) + Mathf.Pow(0.5f - center.y + normalizedSectionSize / 2, 2)) <= sampleRadius &&
               Mathf.Sqrt(Mathf.Pow(0.5f - center.x + normalizedSectionSize / 2, 2) + Mathf.Pow(0.5f - center.y + normalizedSectionSize / 2, 2)) <= sampleRadius &&
               Mathf.Sqrt(Mathf.Pow(0.5f - center.x + normalizedSectionSize / 2, 2) + Mathf.Pow(0.5f - center.y - normalizedSectionSize / 2, 2)) <= sampleRadius &&
               Mathf.Sqrt(Mathf.Pow(0.5f - center.x - normalizedSectionSize / 2, 2) + Mathf.Pow(0.5f - center.y - normalizedSectionSize / 2, 2)) <= sampleRadius;
    }

    public Vector2Int GetWaferSectionLocationFromChunk(ChunkCoordinate chunkCoordinate)
    {
        var sectionRow = chunkCoordinate.chunkRow / numChunksPerSectionLength;
        var sectionCol = chunkCoordinate.chunkCol / numChunksPerSectionLength;
        return new Vector2Int(sectionRow, sectionCol);
    }
}