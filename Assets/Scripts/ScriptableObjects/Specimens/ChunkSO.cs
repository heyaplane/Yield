using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Codice.Client.BaseCommands;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

[CreateAssetMenu(fileName = "ChunkSO", menuName = "Scriptable Object/Specimen/Chunk")]
public class ChunkSO : ScriptableObject
{
    [SerializeField] AssetReferenceSprite[] spriteRefs;

    [SerializeField] int chunkSize;
    public int ChunkSize => chunkSize;

    [SerializeField] int spriteSize;
    public int SpriteSize => spriteSize;

    [SerializeField] int pixelsPerUnit;
    public int PixelsPerUnit => pixelsPerUnit;

    public int CoordinateWorldSize => spriteSize / pixelsPerUnit;

    Sprite[,] spriteArray;
    object lockObject;
    object loadUnloadLockObject = new object();
    int loadedCount;
    List<AsyncOperationHandle> handles;

    public IEnumerator LoadSprites(Action<Sprite[,]> onSpritesLoaded)
    {
        lock (loadUnloadLockObject)
        {
            if (handles != null && handles.Any(x => x.IsValid()))
            {
                Debug.LogWarning("Tried Loading when handles were still valid.");
                while (handles.Any(x => x.IsValid()))
                {
                    yield return null;
                }
                Debug.LogWarning("Resolved.");
            }
            
            loadedCount = 0;
            lockObject = new object();
            spriteArray = new Sprite[chunkSize, chunkSize];
            handles = new List<AsyncOperationHandle>();

            for (int i = 0; i < spriteRefs.Length; i++)
            {
                var spriteRef = spriteRefs[i];
                if (spriteRef == null)
                {
                    Debug.LogError("One of the sprite references was null!");
                    loadedCount++;
                    continue;
                }

                int index = i;
                var handle = spriteRef.LoadAssetAsync<Sprite>();
                handles.Add(handle);
                handle.Completed += _handle =>
                {
                    if (_handle.Status == AsyncOperationStatus.Succeeded)
                    {
                        int row = index / chunkSize;
                        int col = index % chunkSize;
                        spriteArray[row, col] = _handle.Result;
                        CheckIfAllSpritesLoaded(onSpritesLoaded);
                    }

                    else
                    {
                        Debug.LogError($"Failed to load sprite at position {index}.");
                    }
                };
            }
        }
    }

    void CheckIfAllSpritesLoaded(Action<Sprite[,]> onSpritesLoaded)
    {
        lock (lockObject)
        {
            loadedCount++;
            if (loadedCount >= spriteRefs.Length)
                onSpritesLoaded?.Invoke(spriteArray);
        }
    }

    public void UnloadSprites()
    {
        lock (loadUnloadLockObject)
        {
            foreach (var handle in handles)
            {
                Addressables.Release(handle);
            }
        }
    }
}