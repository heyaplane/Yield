using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;

public class FileSystemManager : SingletonMonobehaviour<FileSystemManager>
{
    public VirtualDirectory RootDirectory { get; private set; }

    LRUCache cache;

    void OnEnable()
    {
        // This will always run, and be overwritten later if a save file exists.
        RootDirectory = new VirtualDirectory("/");
        cache = new LRUCache(1);
    }

    void OnDestroy()
    {
        RootDirectory.DestroyUnsavedPersistentFiles();
    }

    public VirtualDirectory FindDirectoryInRoot(string directoryName) => RootDirectory.FindFile(directoryName) as VirtualDirectory;

    public bool TrySaveFile(string waferID, IVirtualFile newFile, string sectionName = null)
    {
        VirtualDirectory saveDirectory;
        if (RootDirectory.FindFile(waferID) is not VirtualDirectory waferDirectory)
        {
            waferDirectory = new VirtualDirectory(waferID);
            RootDirectory.AddFile(waferDirectory);
        }
        
        saveDirectory = waferDirectory;

        if (sectionName != null)
        {
            if (waferDirectory.FindFile(sectionName) is not VirtualDirectory sectionDirectory)
            {
                sectionDirectory = new VirtualDirectory(sectionName);
                waferDirectory.AddFile(sectionDirectory);
            }
        
            saveDirectory = sectionDirectory;
        }

        if (saveDirectory.DirectoryFiles.FirstOrDefault(x => x.FileName == newFile.FileName) != null)
            return false;
        
        saveDirectory.AddFile(newFile);
        newFile.SavePersistentFile();
        return true;
    }

    public List<IVirtualFile> GetFilesFromNames(IEnumerable<string> fileNames)
    {
        var files = new List<IVirtualFile>();
        foreach (string fileName in fileNames)
        {
            files.Add(GetFileFromName(fileName));
        }

        return files;
    }

    public IVirtualFile GetFileFromName(string fileName)
    {
        var fileParts = fileName.Split('\\');
        return FindDirectoryInRoot(fileParts[0]).FindFile(fileName);
    }

    public void AddTextureToCache(string key, Texture2D texture)
    {
        cache.Put(key, texture);
    }

    public void GetTextureOnDisk(string filePath, Action<Texture2D> onTextureFound)
    {
        if (!cache.TryGet(filePath, out Texture2D texture))
        {
            StartCoroutine(LoadTextureAsync(filePath, tex => OnTextureLoaded(filePath, tex, onTextureFound)));
            return;
        }

        onTextureFound?.Invoke(texture);
    }

    void OnTextureLoaded(string filePath, Texture2D texture, Action<Texture2D> onTextureFound)
    {
        cache.Put(filePath, texture);
        onTextureFound?.Invoke(texture);
    }

    IEnumerator LoadTextureAsync(string filePath, Action<Texture2D> onTextureLoaded)
    {
        // yield one frame so that the preview object can deactivate before unloading the texture
        yield return null;
        using (UnityWebRequest uwr = UnityWebRequestTexture.GetTexture("file:///" + filePath))
        {
            yield return uwr.SendWebRequest();
            if (uwr.result != UnityWebRequest.Result.Success)
                Debug.LogError("Error: " + uwr.error);
            else
            {
                var texture = DownloadHandlerTexture.GetContent(uwr);
                onTextureLoaded?.Invoke(texture);
            }
        }
    }

    public object CaptureSaveData() => RootDirectory.GetSerializableFile();

    public void RestoreSaveData(object saveData)
    {
        if (saveData is SerializedFile file)
            RootDirectory = new VirtualDirectory(file);
    }
}
