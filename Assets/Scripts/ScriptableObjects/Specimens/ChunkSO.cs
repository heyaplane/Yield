using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

[CreateAssetMenu(fileName = "ChunkSO", menuName = "Scriptable Object/Specimen/Chunk")]
public class ChunkSO : ScriptableObject
{
    [SerializeField] AssetReferenceSprite[] spriteRefs;

    [SerializeField] int chunkSize;

    Sprite[,] spriteArray;
    object lockObject = new object();
    int loadedCount;
    List<AsyncOperationHandle> handles;

    public void LoadSprites(Action<Sprite[,]> onSpritesLoaded)
    {
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
        foreach (var sprite in handles)
        {
            Addressables.Release(sprite);
        }
    }
}