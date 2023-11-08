using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Pool;

public class MapViewManager : MonoBehaviour
{
    [SerializeField] int randomSeed;
    MapDataManager mapDataManager;

    [SerializeField] CoordinateRenderer CoordinateRendererPrefab;
    ObjectPool<CoordinateRenderer> coordinateRendererPool;

    [SerializeField] ContactFilter2D coordinateFilter;
    [SerializeField] SpriteMask spriteMask;
    List<Collider2D> colliderResults;
    
    List<ChunkCoordinate> previousVisibleCoordinates;
    List<ChunkCoordinate> currentVisibleCoordinates;

    HashSet<ChunkCoordinate> previousFringeCoordinates;
    HashSet<ChunkCoordinate> currentFringeCoordinates;
    HashSet<(int, int)> chunksToLoad;

    [SerializeField] MapSO currentMapSO;
    [SerializeField] Transform coordinateParent;

    Dictionary<ChunkCoordinate, CoordinateRenderer> rendererLookup;
    
    void Awake()
    {
        mapDataManager = new MapDataManager(randomSeed);

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
        chunksToLoad = new HashSet<(int, int)>();
    }

    IEnumerator Start()
    {
        mapDataManager.InitializeMap(currentMapSO);
        var startingCoord = new ChunkCoordinate(2, 2, 1, 1, spriteMask.transform.position);
        
        mapDataManager.ActivateChunks(new HashSet<(int, int)>{(2, 2)});

        var waitCondition = new WaitUntil(() => mapDataManager.HasLoadedSprites(startingCoord));

        yield return waitCondition;
        
        var initRenderer = coordinateRendererPool.Get();
        rendererLookup[startingCoord] = initRenderer;
        var sprite = mapDataManager.GetCoordinateSprite(startingCoord);
        rendererLookup[startingCoord].Initialize(startingCoord, sprite);
        
        Update();
        coordinateRendererPool.Release(initRenderer);
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
                var sprite = mapDataManager.GetCoordinateSprite(coordinate);
                rendererLookup[coordinate].Initialize(coordinate, sprite);
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

        UpdateChunkData(currentVisibleCoordinates);
        
        previousFringeCoordinates.Clear();
        currentFringeCoordinates.Clear();
        
        foreach (var coordinate in previousVisibleCoordinates)
        {
            var currentLocation = rendererLookup[coordinate].transform.position;
            previousFringeCoordinates.UnionWith(coordinate.GetNeighboringCoordinates(currentMapSO, currentLocation));
        }

        foreach (var coordinate in currentVisibleCoordinates)
        {
            var currentLocation = rendererLookup[coordinate].transform.position;
            currentFringeCoordinates.UnionWith(coordinate.GetNeighboringCoordinates(currentMapSO, currentLocation));
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

    void UpdateChunkData(List<ChunkCoordinate> currentlyVisibleCoordinates)
    {
        chunksToLoad.Clear();
        foreach (var coordinate in currentlyVisibleCoordinates)
        {
            chunksToLoad.UnionWith(coordinate.GetNeighboringChunks(currentMapSO));
        }
        
        mapDataManager.ActivateChunks(chunksToLoad);
    }
}