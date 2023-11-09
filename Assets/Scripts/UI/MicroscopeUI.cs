using System;
using UnityEngine;
using UnityEngine.UI;

public class MicroscopeUI : MonoBehaviour
{
    [SerializeField] MapViewManager mapViewManager;
    
    [SerializeField] Transform map;

    [SerializeField] Button lowResButton;
    [SerializeField] Button medResButton;
    [SerializeField] Button highResButton;
    [SerializeField] Button measurementButton;

    [SerializeField] float threshold;
    [SerializeField] Slider focusSlider;
    [SerializeField] Material blurMaterial;
    static readonly int BlurAmount = Shader.PropertyToID("_BlurAmount");

    [SerializeField] float moveMultiplier;

    [SerializeField] MeasurementLine measurementLinePrefab;
    MeasurementLine currentMeasurement;
    
    void OnEnable()
    {
        lowResButton.onClick.AddListener(() => StartCoroutine(mapViewManager.SwitchToNewResolution(ChunkResolution.Low)));
        medResButton.onClick.AddListener(() => StartCoroutine(mapViewManager.SwitchToNewResolution(ChunkResolution.Med)));
        highResButton.onClick.AddListener(() => StartCoroutine(mapViewManager.SwitchToNewResolution(ChunkResolution.High)));
        measurementButton.onClick.AddListener(HandleMeasurement);
    }

    void OnDisable()
    {
        lowResButton.onClick.RemoveAllListeners();
        medResButton.onClick.RemoveAllListeners();
        highResButton.onClick.RemoveAllListeners();
        measurementButton.onClick.RemoveAllListeners();
    }

    void Update()
    {
        float currentFocus = focusSlider.value;
        if (currentFocus < threshold && currentFocus >= 0)
            currentFocus = threshold;
        if (currentFocus > -threshold && currentFocus < 0)
            currentFocus = -threshold;
        blurMaterial.SetFloat(BlurAmount, currentFocus);

        var sampleMove = ControlsManager.Instance.SampleMoveVector * moveMultiplier;
        map.position -= (Vector3) sampleMove;
    }

    // void SwitchSprite(Sprite newSprite)
    // {
    //     var currentOffset = (Vector2) spriteRenderer.transform.position - maskLocation;
    //     float widthScale = newSprite.bounds.size.x / spriteRenderer.sprite.bounds.size.x;
    //     float heightScale = newSprite.bounds.size.y / spriteRenderer.sprite.bounds.size.y;
    //     var newOffset = new Vector2(currentOffset.x * widthScale, currentOffset.y * heightScale);

    //     spriteRenderer.sprite = newSprite;

    //     spriteRenderer.transform.position = new Vector2(maskLocation.x + newOffset.x, maskLocation.y + newOffset.y);
    // }

    void HandleMeasurement()
    {
        if (currentMeasurement != null)
            Destroy(currentMeasurement.gameObject);
        currentMeasurement = Instantiate(measurementLinePrefab);
        // currentMeasurement.Initialize(spriteRenderer.sprite);
    }
}
