using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Pool;

public class MapManager : MonoBehaviour
{
    [SerializeField] int randomSeed;
    ChunkManager chunkManager;

    int[] randomChunkArray = new int[5];

    [SerializeField] CoordinateRenderer CoordinateRendererPrefab;
    ObjectPool<CoordinateRenderer> coordinateRendererPool;

    [SerializeField] ContactFilter2D coordinateFilter;
    [SerializeField] SpriteMask spriteMask;
    List<Collider2D> colliderResults;
    
    List<ChunkCoordinate> previousVisibleCoordinates;
    List<ChunkCoordinate> currentVisibleCoordinates;

    HashSet<ChunkCoordinate> previousFringeCoordinates;
    HashSet<ChunkCoordinate> currentFringeCoordinates;

    [SerializeField] Sprite testSprite;
    [SerializeField] MapSO currentMapSO;
    int[,] currentMapData;
    [SerializeField] Transform coordinateParent;

    Dictionary<ChunkCoordinate, CoordinateRenderer> rendererLookup;
    
    void Awake()
    {
        chunkManager = new ChunkManager(randomSeed);

        coordinateRendererPool = new ObjectPool<CoordinateRenderer>
        (
            () => Instantiate(CoordinateRendererPrefab, coordinateParent),
            obj => obj.gameObject.SetActive(true),
            obj => obj.gameObject.SetActive(false),
            Destroy, true, 20, 50
        );
        
        colliderResults = new List<Collider2D>();
        previousVisibleCoordinates = new List<ChunkCoordinate>();
        currentVisibleCoordinates = new List<ChunkCoordinate>();

        rendererLookup = new Dictionary<ChunkCoordinate, CoordinateRenderer>();
        previousFringeCoordinates = new HashSet<ChunkCoordinate>();
        currentFringeCoordinates = new HashSet<ChunkCoordinate>();
    }

    void Start()
    {
        currentMapData = InitializeMap(currentMapSO);
        var startingCoord = new ChunkCoordinate(1, 1, 1, 1, spriteMask.transform.position);
        
        var initRenderer = coordinateRendererPool.Get();
        rendererLookup[startingCoord] = initRenderer;
        rendererLookup[startingCoord].Initialize(startingCoord, testSprite);
        
        Update();
        coordinateRendererPool.Release(initRenderer);
    }

    int[,] InitializeMap(MapSO mapSO)
    {
        var newMap = new int[mapSO.NumRows, mapSO.NumCols];
        for (int i = 0; i < mapSO.NumCols; i++)
        {
            for (int j = 0; j < mapSO.NumRows; j++)
            {
                newMap[i, j] = -1;
            }
        }
        
        foreach (var coord in mapSO.ChunkCoordinates)
        {
            newMap[coord.rowNum, coord.colNum] = coord.chunkNum;
        }
        
        chunkManager.PopulateMap(newMap, randomChunkArray.Length);
        return newMap;
    }

    void Update()
    {
        IEnumerable<ChunkCoordinate> expiredCoordinates, newCoordinates;
        (expiredCoordinates, newCoordinates) = GetCoordinateDiff();
        if (expiredCoordinates == null && newCoordinates == null) return;

        if (expiredCoordinates != null)
        {
            foreach (var coordinate in expiredCoordinates)
            {
                coordinateRendererPool.Release(rendererLookup[coordinate]);
                rendererLookup.Remove(coordinate);
            }
        }

        if (newCoordinates != null)
        {
            foreach (var coordinate in newCoordinates)
            {
                rendererLookup[coordinate] = coordinateRendererPool.Get();
                rendererLookup[coordinate].Initialize(coordinate, testSprite);
            }
        }
    }

    // Returns the out-of-sight coordinates that need to be loaded/unloaded from the edge
    public (IEnumerable<ChunkCoordinate>, IEnumerable<ChunkCoordinate>) GetCoordinateDiff()
    {
        //These are the coordinates that were/are being shown through the sprite mask.
        previousVisibleCoordinates.Clear();
        previousVisibleCoordinates.AddRange(currentVisibleCoordinates);
        
        currentVisibleCoordinates.Clear();
        UpdateAndSortVisibleCoordinates(currentVisibleCoordinates);

        if (previousVisibleCoordinates.SequenceEqual(currentVisibleCoordinates))
            return (null, null);
        
        previousFringeCoordinates.Clear();
        currentFringeCoordinates.Clear();
        
        foreach (var coordinate in previousVisibleCoordinates)
        {
            var currentLocation = rendererLookup[coordinate].transform.position;
            previousFringeCoordinates.UnionWith(coordinate.GetSurroundingCoordinates(currentMapSO, currentLocation));
        }

        foreach (var coordinate in currentVisibleCoordinates)
        {
            var currentLocation = rendererLookup[coordinate].transform.position;
            currentFringeCoordinates.UnionWith(coordinate.GetSurroundingCoordinates(currentMapSO, currentLocation));
        }

        var coordinatesToRemove = previousFringeCoordinates.Except(currentFringeCoordinates);
        var coordinatesToAdd = currentFringeCoordinates.Except(previousFringeCoordinates);

        return (coordinatesToRemove, coordinatesToAdd);
    }

    void UpdateAndSortVisibleCoordinates(List<ChunkCoordinate> coordinateList)
    {
        Physics2D.OverlapBox(spriteMask.transform.position, spriteMask.bounds.size, 0f, coordinateFilter, colliderResults);
        foreach (var result in colliderResults)
        {
            if (result.TryGetComponent(out CoordinateRenderer coordinateRenderer))
                coordinateList.Add(coordinateRenderer.CurrentCoordinate);
        }

        coordinateList.Sort();
    }
}