using System;
using System.Globalization;
using System.IO;
using Newtonsoft.Json;
using UnityEngine;

public class VirtualImage : IGeneratePreview
{
    public string FileName { get; set; }
    string filePath => Path.Combine(Application.persistentDataPath, SaveManager.Instance.CurrentPlayerProfile, "saves", SaveManager.Instance.CurrentSaveGame, FileName);
    public DateTime CreationDateTime { get; }
    public DateTime LastModifiedDateTime { get; set; }
    public int FileSize { get; }
    public float MeasurementValue { get; }
    
    Texture2D image;
    bool deletePersistentFileOnDestroy = true;

    const int bytesPerPixel = 4;

    public VirtualImage(string fileName, Texture2D image, float measurementValue = -1f)
    {
        FileName = fileName;
        this.image = image;
        CreationDateTime = DateTime.Now;
        LastModifiedDateTime = DateTime.Now;
        FileSize = EstimateImageFileSize(image);
        MeasurementValue = measurementValue;
    }

    public VirtualImage(SerializedFile file)
    {
        FileName = file.FileName;
        CreationDateTime = file.CreationDateTime;
        LastModifiedDateTime = file.LastModifiedDateTime;
        FileSize = file.FileSize;
        MeasurementValue = (float) (double)file.AdditionalData;
        deletePersistentFileOnDestroy = false;
    }

    int EstimateImageFileSize(Texture2D img)
    {
        if (img == null) return 0;
        
        int totalSize = 0;
        int width = img.width;
        int height = img.height;
        
        for (int i = 0; i < img.mipmapCount; i++)
        {
            totalSize += width * height * bytesPerPixel;
            width /= 2;
            height /= 2;
        }

        return totalSize;
    }

    public void GetPreviewImage(Action<Texture2D> onTextureFound)
    {
        if (image != null) onTextureFound?.Invoke(image);
        else
        {
            void OnTextureFound(Texture2D texture)
            {
                image = texture;
                onTextureFound?.Invoke(texture);
            }
            
            FileSystemManager.Instance.GetTextureOnDisk(filePath, OnTextureFound);
        }
    }
    
    public void SavePersistentFile()
    {
        if (image == null)
            Debug.LogError("Tried to save a null texture!");

        var bytes = image.EncodeToPNG();
        if (SaveSystemHelpers.CheckIfDirectoryAndFileExist(filePath))
            Debug.LogError("Image will overwrite existing image on disk. This should not happen.");
        else
        {
            File.WriteAllBytes(filePath, bytes);
            FileSystemManager.Instance.AddTextureToCache(filePath, image);
        }
    }

    public void DestroyUnsavedPersistentFiles()
    {
        if (!deletePersistentFileOnDestroy) return;
        
        if (File.Exists(filePath))
            File.Delete(filePath);
    }

    public SerializedFile GetSerializableFile()
    {
        deletePersistentFileOnDestroy = false;
        return new SerializedFile
        {
            FileName = FileName,
            CreationDateTime = CreationDateTime,
            LastModifiedDateTime = LastModifiedDateTime,
            FileSize = FileSize,
            FileType = TypeOfFile.Image,
            AdditionalData = MeasurementValue
        };
    }
}
