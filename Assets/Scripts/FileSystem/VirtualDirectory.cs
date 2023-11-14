using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;

public class VirtualDirectory : IVirtualFile
{
    public string FileName { get; set; }
    public DateTime CreationDateTime { get; }
    public DateTime LastModifiedDateTime { get; set; }
    public int FileSize { get; private set; }
    public Texture2D Image => null;
    
    public List<IVirtualFile> DirectoryFiles { get; private set; }
    public string[] DirectoryFileNames => DirectoryFiles.Select(x => x.FileName).ToArray();

    public VirtualDirectory(string directoryName)
    {
        FileName = directoryName;
        CreationDateTime = DateTime.Now;
        LastModifiedDateTime = DateTime.Now;
        FileSize = 0;
        DirectoryFiles = new List<IVirtualFile>();
    }

    public VirtualDirectory(SerializedFile serializedFile)
    {
        FileName = serializedFile.FileName;
        CreationDateTime = serializedFile.CreationDateTime;
        LastModifiedDateTime = serializedFile.LastModifiedDateTime;
        FileSize = serializedFile.FileSize;
        DirectoryFiles = new List<IVirtualFile>();

        //if (serializedFile.AdditionalData is not List<SerializedFile> directoryFiles) return;
        var directoryFiles = (serializedFile.AdditionalData as JArray)?.ToObject<List<SerializedFile>>();
        if (directoryFiles == null) return;
        
        foreach (var file in directoryFiles)
        {
            switch (file.FileType)
            {
                case TypeOfFile.Directory:
                    DirectoryFiles.Add(new VirtualDirectory(file));
                    break;
                case TypeOfFile.Image:
                    DirectoryFiles.Add(new VirtualImage(file));
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }

    public void AddFile(IVirtualFile file)
    {
        DirectoryFiles.Add(file);
        FileSize += file.FileSize;
        LastModifiedDateTime = DateTime.Now;
    }

    public void RemoveFile(IVirtualFile file)
    {
        if (DirectoryFiles.Remove(file))
        {
            FileSize -= file.FileSize;
            LastModifiedDateTime = DateTime.Now;
        }
    }

    public IVirtualFile FindFile(string fileName) => DirectoryFiles.FirstOrDefault(x => x.FileName == fileName);
    
    public void SavePersistentFile() {}

    public void DestroyUnsavedPersistentFiles() => DirectoryFiles.ForEach(x => x.DestroyUnsavedPersistentFiles());

    public SerializedFile GetSerializableFile() => 
        new SerializedFile
        {
            FileName   = FileName,
            CreationDateTime = CreationDateTime,
            LastModifiedDateTime = LastModifiedDateTime,
            FileSize = FileSize,
            FileType = TypeOfFile.Directory,
            AdditionalData = DirectoryFiles.Select(x => x.GetSerializableFile()).ToList()
        };
}

[Serializable]
public class SerializedFile
{
    public string FileName;
    public DateTime CreationDateTime;
    public DateTime LastModifiedDateTime;
    public int FileSize;
    public TypeOfFile FileType;
    public object AdditionalData;
}

public enum TypeOfFile
{
    Directory, Image
}
