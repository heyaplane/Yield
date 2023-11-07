using System.Collections.Generic;
using UnityEngine;

public class ChunkManager
{
    public Dictionary<(int, int), Sprite[,]> chunkLookup;
    
    public ChunkManager(int seed)
    {
        Random.InitState(seed);

        chunkLookup = new Dictionary<(int, int), Sprite[,]>();
    }
    
    public void PopulateMap(int[,] chunkArray, int numRandomChunks)
    {
        int width = chunkArray.GetLength(0);
        int height = chunkArray.GetLength(1);
        
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (chunkArray[i, j] != -1) continue;
                chunkArray[i, j] = Random.Range(0, numRandomChunks);
            }
        }
    }

    public Sprite GetCoordinateSprite(ChunkCoordinate coordinate)
    {
        var images = chunkLookup[(coordinate.chunkRow, coordinate.chunkCol)];
        return images[coordinate.imageRow, coordinate.imageCol];
    }
}
