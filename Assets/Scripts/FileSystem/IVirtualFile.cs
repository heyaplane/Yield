using System;
using UnityEngine;

public interface IVirtualFile
{
    string FileName { get; set; }
    DateTime CreationDateTime { get; }
    DateTime LastModifiedDateTime { get; set; }
    int FileSize { get; }
    void SavePersistentFile();
    SerializedFile GetSerializableFile();
}
