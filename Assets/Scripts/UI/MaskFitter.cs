using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(SpriteMask))]
public class MaskFitter : MonoBehaviour
{
    [SerializeField] RectTransform microscopeViewTransform;
    [SerializeField] CanvasScaler canvasScaler;
    
    void Start()
    {
        var mainCamera = Camera.main;
        var sizeInPixels = microscopeViewTransform.rect.size;

        // Calculate frustum height
        float frustumHeight = 2.0f * mainCamera.orthographicSize;
        // Calculate frustum width based on the aspect ratio
        float frustumWidth = frustumHeight * mainCamera.aspect;

        // Scale the mask
        Vector3 scale;
        scale.x = sizeInPixels.x / canvasScaler.referenceResolution.x * frustumWidth;
        scale.y = sizeInPixels.y / canvasScaler.referenceResolution.y * frustumHeight;
        scale.z = 1; // Assuming the mask only needs scaling in x and y

        transform.localScale = scale;
        transform.position = new Vector3(microscopeViewTransform.position.x, microscopeViewTransform.position.y, -1);


        Vector2 renderTextureScale;
        renderTextureScale.x = sizeInPixels.x / canvasScaler.referenceResolution.x * Screen.width;
        renderTextureScale.y = sizeInPixels.y / canvasScaler.referenceResolution.y * Screen.height;
        RenderCameraManager.Instance.SetCameraAndTextureBounds(new Bounds(transform.position, renderTextureScale), transform.localScale.y);
    }
}