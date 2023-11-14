using System;
using UnityEngine;

public interface IGeneratePreview : IVirtualFile
{
    void GetPreviewImage(Action<Texture2D> onTextureFound);
}
