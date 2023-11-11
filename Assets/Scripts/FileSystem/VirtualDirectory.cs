using System;
using System.Collections.Generic;
using System.Linq;
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
}
