using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class MapDataManager
{
    Dictionary<ChunkSO, Sprite[,]> spriteLookup;
    Dictionary<(int, int), ChunkGroupSO> chunkGroupLookup;

    HashSet<ChunkSO> activatedChunkSO;
    HashSet<ChunkSO> chunkSOToActivate;
    HashSet<(int, int)> chunksToLoad;

    public ChunkResolution CurrentChunkResolution { get; private set; }

    public ChunkSO GetChunkFromCoordinate(ChunkCoordinate coordinate) => 
        chunkGroupLookup[(coordinate.chunkRow, coordinate.chunkCol)].GetChunkFromResolution(CurrentChunkResolution);

    public bool HasLoadedSprites(ChunkCoordinate coordinate)
    {
        var chunkGroupSO = chunkGroupLookup[(coordinate.chunkRow, coordinate.chunkCol)];
        return spriteLookup.TryGetValue(chunkGroupSO.GetChunkFromResolution(CurrentChunkResolution), out var sprites) && sprites != null;
    }

    int numChunksToLoad;
    object lockObject = new object();
    public bool HasLoadedAllSprites() => numChunksToLoad == 0;

    public MapDataManager(int seed)
    {
        Random.InitState(seed);

        spriteLookup = new Dictionary<ChunkSO, Sprite[,]>();
        chunkGroupLookup = new Dictionary<(int, int), ChunkGroupSO>();
        activatedChunkSO = new HashSet<ChunkSO>();
        chunkSOToActivate = new HashSet<ChunkSO>();
        chunksToLoad = new HashSet<(int, int)>();
    }
    
    public void InitializeMap(MapSO mapSO)
    {
        foreach (var coord in mapSO.ChunkGroupAssignments)
        {
            chunkGroupLookup[(coord.rowNum, coord.colNum)] = coord.chunkData;
        }
        
        PopulateMapWithRandomChunks(mapSO);
    }
    
    public void PopulateMapWithRandomChunks(MapSO mapSO)
    {
        for (int i = 0; i < mapSO.NumRows; i++)
        {
            for (int j = 0; j < mapSO.NumCols; j++)
            {
                if (chunkGroupLookup.TryGetValue((i,j), out var chunkData)) continue;
                chunkGroupLookup[(i, j)] = mapSO.RandomChunkPool[Random.Range(0, mapSO.RandomChunkPool.Length)];
            }
        }
    }

    public void SwitchToNewResolution(ChunkResolution newResolution, ChunkCoordinate startingChunk, MapSO currentMapSO)
    {
        CurrentChunkResolution = newResolution;
        
        // Activating a single new chunk at a new resolution will automatically unload other chunks and replace the current chunk sprites
        UpdateChunkData(new List<ChunkCoordinate>{startingChunk}, currentMapSO);
    }

    public Sprite GetCoordinateSprite(ChunkCoordinate coordinate)
    {
        var chunk = chunkGroupLookup[(coordinate.chunkRow, coordinate.chunkCol)].GetChunkFromResolution(CurrentChunkResolution);
        if (spriteLookup.TryGetValue(chunk, out var images)) 
            return images[coordinate.imageRow, coordinate.imageCol];
        
        Debug.LogError("Tried to access sprites that have not been loaded!");
        return null;
    }
    
    public void UpdateChunkData(List<ChunkCoordinate> currentlyVisibleCoordinates, MapSO currentMapSO)
    {
        chunksToLoad.Clear();
        foreach (var coordinate in currentlyVisibleCoordinates)
        {
            chunksToLoad.UnionWith(coordinate.GetNeighboringChunks(currentMapSO));
        }
        
        ActivateChunks(chunksToLoad);
    }

    void ActivateChunks(HashSet<(int, int)> chunksToActivate)
    {
        chunkSOToActivate.Clear();
        
        foreach (var chunk in chunksToActivate)
        {
            chunkSOToActivate.Add(chunkGroupLookup[chunk].GetChunkFromResolution(CurrentChunkResolution));
        }

        var chunkSOToUnload = activatedChunkSO.Except(chunkSOToActivate).ToList();
        var chunkSOToLoad = chunkSOToActivate.Except(activatedChunkSO).ToList();

        foreach (var chunkSO in chunkSOToUnload)
        {
            UnloadChunk(chunkSO);
        }

        foreach (var chunkSO in chunkSOToLoad)
        {
            lock (lockObject)
            {
                numChunksToLoad++;
            }
            LoadChunk(chunkSO);
        }
    }

    public void UnloadAllChunks()
    {
        foreach (var chunkSO in activatedChunkSO)
        {
            if (spriteLookup.TryGetValue(chunkSO, out var value))
                spriteLookup[chunkSO] = null;
            chunkSO.UnloadSprites();
        }

        activatedChunkSO.Clear();
    }

    void UnloadChunk(ChunkSO chunkSO)
    {
        spriteLookup[chunkSO] = null;
        chunkSO.UnloadSprites();
        activatedChunkSO.Remove(chunkSO);
        Debug.Log("Asset(s) unloaded.");
    }

    void LoadChunk(ChunkSO chunkSO)
    {
        void SpritesLoaded(Sprite[,] sprites)
        {
            spriteLookup[chunkSO] = sprites;
            lock (lockObject)
            {
                numChunksToLoad--;
            }
            
            Debug.Log("Asset(s) loaded.");
        }
        
        GameManager.Instance.StartCoroutine(chunkSO.LoadSprites(SpritesLoaded));
        activatedChunkSO.Add(chunkSO);
    }
}
