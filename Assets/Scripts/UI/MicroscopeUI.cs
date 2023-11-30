using System.Collections;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MicroscopeUI : BaseUI
{
    [SerializeField] WaferMapViewManager waferMapViewManager;
    [SerializeField] MapViewClickListener mapViewClickListener;
    
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

    [SerializeField] Button saveFileButton;
    [SerializeField] TextMeshProUGUI waferIDText;
    [SerializeField] TextMeshProUGUI sectionLocationText;
    [SerializeField] TMP_InputField imageNameInput;
    [SerializeField] TMP_InputField nextSuffixInput;
    [SerializeField] TextMeshProUGUI exampleText;

    [SerializeField] Button closeUIButton;

    [SerializeField] WaferSectionMicroscopeMapUI waferSectionMap;

    string currentWaferID, currentSectionName, currentImageName, currentSuffix;

    void OnEnable()
    {
        lowResButton.onClick.AddListener(() => StartCoroutine(waferMapViewManager.SwitchToNewResolution(ChunkResolution.Low)));
        medResButton.onClick.AddListener(() => StartCoroutine(waferMapViewManager.SwitchToNewResolution(ChunkResolution.Med)));
        highResButton.onClick.AddListener(() => StartCoroutine(waferMapViewManager.SwitchToNewResolution(ChunkResolution.High)));
        measurementButton.onClick.AddListener(HandleMeasurement);
        closeUIButton.onClick.AddListener(HandleCloseUIButton);
        
        saveFileButton.onClick.AddListener(HandleSaveFile);
        imageNameInput.onValueChanged.AddListener(HandleImageNameChanged);
        nextSuffixInput.onValueChanged.AddListener(HandleSuffixChanged);

        currentImageName = (imageNameInput.placeholder as TextMeshProUGUI)?.text;
        currentSuffix = (nextSuffixInput.placeholder as TextMeshProUGUI)?.text;

        UpdateExampleName();
    }

    void OnDisable()
    {
        lowResButton.onClick.RemoveAllListeners();
        medResButton.onClick.RemoveAllListeners();
        highResButton.onClick.RemoveAllListeners();
        measurementButton.onClick.RemoveAllListeners();
        closeUIButton.onClick.RemoveAllListeners();
        
        saveFileButton.onClick.RemoveAllListeners();
        imageNameInput.onValueChanged.RemoveAllListeners();
        nextSuffixInput.onValueChanged.RemoveAllListeners();
    }

    public override void EnableWindow()
    {
        base.EnableWindow();
        currentWaferID = WaferManager.Instance.ActiveWafer.WaferName;
        waferIDText.text = currentWaferID;
        waferMapViewManager.CurrentWaferMapSO = WaferManager.Instance.ActiveWafer.WaferMap;
        waferMapViewManager.gameObject.SetActive(true);
        mapViewClickListener.gameObject.SetActive(true);
        StartCoroutine(WaitForView());
        
        waferSectionMap.Initialize();
    }

    IEnumerator WaitForView()
    {
        while (waferMapViewManager.IsSwitchingResolution)
            yield return null;
        
        var coordinate = waferMapViewManager.GetCenterCoordinate();
        UpdateSectionName(coordinate);
    }

    void Update()
    {
        float currentFocus = focusSlider.value;
        if (currentFocus < threshold && currentFocus >= 0)
            currentFocus = threshold;
        if (currentFocus > -threshold && currentFocus < 0)
            currentFocus = -threshold;
        blurMaterial.SetFloat(BlurAmount, currentFocus);

        var sampleMove = ControlsManager.Instance.MapMoveVector * moveMultiplier;
        map.position -= (Vector3) sampleMove;
        if (sampleMove != Vector2.zero)
        {
            var coordinate = waferMapViewManager.GetCenterCoordinate();
            UpdateSectionName(coordinate);
            UpdateWaferMap(coordinate);
        }
    }

    void UpdateSectionName(ChunkCoordinate coordinate)
    {
        currentSectionName = WaferManager.Instance.GetSectionLocationAsStringFromChunk(coordinate);
        sectionLocationText.text = currentSectionName;
        UpdateExampleName();
    }

    void UpdateWaferMap(ChunkCoordinate coordinate)
    {
        waferSectionMap.UpdateMarkerImage(new Vector2((float) coordinate.chunkCol / 256, (float) coordinate.chunkRow / 256));
    }

    void HandleMeasurement() => mapViewClickListener.HandleMeasurementToggle();

    void HandleSaveFile()
    {
        var texture = RenderCameraManager.Instance.RenderNewTexture();
        var newFile = new VirtualImage(Path.Combine($"{currentWaferID}", $"{currentSectionName}",$"{currentImageName}_{currentSuffix}.png"), texture, mapViewClickListener.CurrentMeasurementValue);
        if (!FileSystemManager.Instance.TrySaveFile(currentWaferID, newFile, currentSectionName))
            Debug.LogError("Couldn't save file, duplicate name!");
    }

    public void HandleSampleIDChanged(string sampleID)
    {
    }

    void HandleImageNameChanged(string imageName)
    {
        currentImageName = imageName;
        UpdateExampleName(imageName: imageName);
    }

    void HandleSuffixChanged(string suffix)
    {
        currentSuffix = suffix;
        UpdateExampleName(suffix: suffix);
    }

    void UpdateExampleName(string waferID = null, string sectionName = null, string imageName = null, string suffix = null)
    {
        waferID ??= currentWaferID;
        sectionName ??= currentSectionName;
        imageName ??= currentImageName;
        suffix ??= currentSuffix;
        
        exampleText.text = $@"{waferID}\{sectionName}\{imageName}_{suffix}.png";
    }

    void HandleCloseUIButton()
    {
        waferMapViewManager.gameObject.SetActive(false);
        mapViewClickListener.gameObject.SetActive(false);
        OnCancelAction?.Invoke();
        CloseWindow();
    }
}
