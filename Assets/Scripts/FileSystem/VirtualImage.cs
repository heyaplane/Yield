using System;
using UnityEngine;

public class VirtualImage : IVirtualFile
{
    public string FileName { get; set; }
    public DateTime CreationDateTime { get; }
    public DateTime LastModifiedDateTime { get; set; }
    public int FileSize { get; }
    public byte[] PreviewImage => Img.GetRawTextureData();
    
    public Texture2D Img { get; }
    public float MeasurementValue { get; }

    const int bytesPerPixel = 4;

    public VirtualImage(string fileName, Texture2D img, float measurementValue = -1f)
    {
        FileName = fileName;
        Img = img;
        CreationDateTime = DateTime.Now;
        LastModifiedDateTime = DateTime.Now;
        FileSize = EstimateImageFileSize(img);
        MeasurementValue = measurementValue;
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
}
