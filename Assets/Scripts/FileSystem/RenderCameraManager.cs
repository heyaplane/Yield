using System;
using UnityEngine;

public class RenderCameraManager : SingletonMonobehaviour<RenderCameraManager>
{
    [SerializeField] Camera renderCamera;

    RenderTexture renderTexture;
    
    public void SetCameraAndTextureBounds(Bounds renderTextureBounds, float orthographicSize)
    {
        if (renderTexture != null)
        {
            renderTexture.Release();
            Destroy(renderTexture);
        }
        
        renderTexture = new RenderTexture((int) renderTextureBounds.size.x, (int) renderTextureBounds.size.y, 0);
        renderCamera.targetTexture = renderTexture;

        renderCamera.orthographicSize = orthographicSize;
        renderCamera.transform.position = new Vector3(renderTextureBounds.center.x, renderTextureBounds.center.y, renderCamera.transform.position.z);
    }

    public Texture2D RenderNewTexture()
    {
        RenderTexture.active = renderTexture;
        var capturedTexture = new Texture2D(renderTexture.width, renderTexture.height, TextureFormat.RGBA32, false);
        renderCamera.Render();
        
        capturedTexture.ReadPixels(new Rect(0,0, renderTexture.width, renderTexture.height), 0, 0);
        capturedTexture.Apply();
        RenderTexture.active = null;
        return capturedTexture;
    }
}
