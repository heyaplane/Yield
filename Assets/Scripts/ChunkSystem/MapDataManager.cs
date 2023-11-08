using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class MapDataManager
{
    Dictionary<ChunkSO, Sprite[,]> spriteLookup;
    Dictionary<(int, int), ChunkSO> chunkLookup;

    HashSet<ChunkSO> activatedChunkSO;
    HashSet<ChunkSO> chunkSOToActivate;

    public bool HasLoadedSprites(ChunkCoordinate coordinate)
    {
        var chunkSO = chunkLookup[(coordinate.chunkRow, coordinate.chunkCol)];
        return spriteLookup.TryGetValue(chunkSO, out var sprites) && sprites != null;
    }

    public MapDataManager(int seed)
    {
        Random.InitState(seed);

        spriteLookup = new Dictionary<ChunkSO, Sprite[,]>();
        chunkLookup = new Dictionary<(int, int), ChunkSO>();
        activatedChunkSO = new HashSet<ChunkSO>();
        chunkSOToActivate = new HashSet<ChunkSO>();
    }
    
    public void InitializeMap(MapSO mapSO)
    {
        foreach (var coord in mapSO.ChunkCoordinates)
        {
            chunkLookup[(coord.rowNum, coord.colNum)] = coord.chunkData;
        }
        
        PopulateMapWithRandomChunks(mapSO);
    }
    
    public void PopulateMapWithRandomChunks(MapSO mapSO)
    {
        for (int i = 0; i < mapSO.NumRows; i++)
        {
            for (int j = 0; j < mapSO.NumCols; j++)
            {
                if (chunkLookup.TryGetValue((i,j), out var chunkData)) continue;
                chunkLookup[(i, j)] = mapSO.RandomChunkPool[Random.Range(0, mapSO.RandomChunkPool.Length)];
            }
        }
    }

    public Sprite GetCoordinateSprite(ChunkCoordinate coordinate)
    {
        if (spriteLookup.TryGetValue(chunkLookup[(coordinate.chunkRow, coordinate.chunkCol)], out var images)) 
            return images[coordinate.imageRow, coordinate.imageCol];
        
        Debug.LogError("Tried to access sprites that have not been loaded!");
        return null;
    }

    public void ActivateChunks(HashSet<(int, int)> chunksToActivate)
    {
        chunkSOToActivate.Clear();
        
        foreach (var chunk in chunksToActivate)
        {
            chunkSOToActivate.Add(chunkLookup[chunk]);
        }

        var chunkSOToUnload = activatedChunkSO.Except(chunkSOToActivate).ToList();
        var chunkSOToLoad = chunkSOToActivate.Except(activatedChunkSO).ToList();

        foreach (var chunkSO in chunkSOToUnload)
        {
            spriteLookup[chunkSO] = null;
            chunkSO.UnloadSprites();
            activatedChunkSO.Remove(chunkSO);
            Debug.Log("Aseet(s) unloaded.");
        }

        foreach (var chunkSO in chunkSOToLoad)
        {
            void SpritesLoaded(Sprite[,] sprites) => spriteLookup[chunkSO] = sprites;
            chunkSO.LoadSprites(SpritesLoaded);
            activatedChunkSO.Add(chunkSO);
            Debug.Log("Aseet(s) loaded.");
        }
    }
}
