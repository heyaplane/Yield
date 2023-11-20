using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Pool;

public class WaferMapViewManager : MonoBehaviour
{
    [SerializeField] int randomSeed;
    WaferMapDataManager waferMapDataManager;

    [SerializeField] CoordinateRenderer CoordinateRendererPrefab;
    ObjectPool<CoordinateRenderer> coordinateRendererPool;

    [SerializeField] ContactFilter2D coordinateFilter;
    [SerializeField] SpriteMask spriteMask;
    List<Collider2D> colliderResults;
    
    List<ChunkCoordinate> previousVisibleCoordinates;
    List<ChunkCoordinate> currentVisibleCoordinates;

    HashSet<ChunkCoordinate> previousFringeCoordinates;
    HashSet<ChunkCoordinate> currentFringeCoordinates;

    [SerializeField] WaferMapSO currentWaferMapSo;
    [SerializeField] Transform coordinateParent;

    Dictionary<ChunkCoordinate, CoordinateRenderer> rendererLookup;

    [SerializeField] ChunkResolution startingChunkResolution;

    public float CurrentScaleFactor => currentWaferMapSo.GetCurrentScaleFactor(waferMapDataManager.CurrentChunkResolution);

    bool isSwitchingResolution;
    
    void Awake()
    {
        waferMapDataManager = new WaferMapDataManager(randomSeed);

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

    IEnumerator Start()
    {
        waferMapDataManager.InitializeMap(currentWaferMapSo);
        var startingCoord = new ChunkCoordinate(1, 1, 1, 1, spriteMask.transform.position);
        
        waferMapDataManager.SwitchToNewResolution(startingChunkResolution, startingCoord, currentWaferMapSo);

        var waitCondition = new WaitUntil(() => waferMapDataManager.HasLoadedAllSprites());

        yield return waitCondition;
        
        var initRenderer = coordinateRendererPool.Get();
        rendererLookup[startingCoord] = initRenderer;
        var sprite = waferMapDataManager.GetCoordinateSprite(startingCoord);
        rendererLookup[startingCoord].Initialize(startingCoord, sprite);
        
        Update();
        coordinateRendererPool.Release(initRenderer);
    }

    void Update()
    {
        if (isSwitchingResolution) return;
        
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
                var sprite = waferMapDataManager.GetCoordinateSprite(coordinate);
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

        waferMapDataManager.UpdateChunkData(currentVisibleCoordinates, currentWaferMapSo);
        
        previousFringeCoordinates.Clear();
        currentFringeCoordinates.Clear();
        
        foreach (var coordinate in previousVisibleCoordinates)
        {
            var currentLocation = rendererLookup[coordinate].transform.position;
            previousFringeCoordinates.UnionWith(coordinate.GetNeighboringCoordinates(currentWaferMapSo, 
                waferMapDataManager.GetChunkFromCoordinate(coordinate), currentLocation));
        }

        foreach (var coordinate in currentVisibleCoordinates)
        {
            var currentLocation = rendererLookup[coordinate].transform.position;
            currentFringeCoordinates.UnionWith(coordinate.GetNeighboringCoordinates(currentWaferMapSo, 
                waferMapDataManager.GetChunkFromCoordinate(coordinate), currentLocation));
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

    public IEnumerator SwitchToNewResolution(ChunkResolution newResolution)
    {
        if (newResolution == waferMapDataManager.CurrentChunkResolution) yield break;

        var results = new Collider2D[1]; 
        Physics2D.OverlapPoint(spriteMask.transform.position, coordinateFilter, results);

        if (!results[0].TryGetComponent(out CoordinateRenderer coordinateRenderer))
        {
            Debug.LogError("Error getting coordinate renderer at center of sprite mask.");
            yield break;
        }
        
        Vector2 currentOffset = coordinateRenderer.transform.position - spriteMask.transform.position;
        float scaleFactor = (float) currentWaferMapSo.GetGroupSpriteSize(newResolution) / currentWaferMapSo.GetGroupSpriteSize(waferMapDataManager.CurrentChunkResolution);
        var newOffset = new Vector2(currentOffset.x * scaleFactor, currentOffset.y * scaleFactor);
        var centerCoordinate = coordinateRenderer.CurrentCoordinate;
        centerCoordinate.startingWorldSpacePos = (Vector2) spriteMask.transform.position + newOffset;
        
        foreach (var coordRenderer in rendererLookup.Values)
        {
            coordRenderer.gameObject.SetActive(false);
        }

        Update();

        waferMapDataManager.SwitchToNewResolution(newResolution, centerCoordinate, currentWaferMapSo);
        
        // We dont want update to run while we are loading assets for the new resolution
        isSwitchingResolution = true;
        
        var waitCondition = new WaitUntil(() => waferMapDataManager.HasLoadedAllSprites());
        yield return waitCondition;
        
        var initRenderer = coordinateRendererPool.Get();
        rendererLookup[centerCoordinate] = initRenderer;
        var sprite = waferMapDataManager.GetCoordinateSprite(centerCoordinate);
        rendererLookup[centerCoordinate].Initialize(centerCoordinate, sprite);

        isSwitchingResolution = false;
        
        Update();
        coordinateRendererPool.Release(initRenderer);
    }

    void OnDestroy()
    {
        waferMapDataManager.UnloadAllChunks();
    }
}