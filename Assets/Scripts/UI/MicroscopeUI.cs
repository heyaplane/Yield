using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MicroscopeUI : BaseUI
{
    [SerializeField] MapViewManager mapViewManager;
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

    [SerializeField] ChooseDirectoryUI chooseDirectoryUI;
    [SerializeField] Button chooseDirectoryButton;
    [SerializeField] Button saveFileButton;
    [SerializeField] TMP_InputField sampleIDInput;
    [SerializeField] TMP_InputField imageNameInput;
    [SerializeField] TMP_InputField nextSuffixInput;
    [SerializeField] TextMeshProUGUI exampleText;

    [SerializeField] Button closeUIButton;

    string currentSampleID, currentImageName, currentSuffix;

    void OnEnable()
    {
        lowResButton.onClick.AddListener(() => StartCoroutine(mapViewManager.SwitchToNewResolution(ChunkResolution.Low)));
        medResButton.onClick.AddListener(() => StartCoroutine(mapViewManager.SwitchToNewResolution(ChunkResolution.Med)));
        highResButton.onClick.AddListener(() => StartCoroutine(mapViewManager.SwitchToNewResolution(ChunkResolution.High)));
        measurementButton.onClick.AddListener(HandleMeasurement);
        closeUIButton.onClick.AddListener(HandleCloseUIButton);
        
        chooseDirectoryButton.onClick.AddListener(HandleOpenFile);
        saveFileButton.onClick.AddListener(HandleSaveFile);
        sampleIDInput.onValueChanged.AddListener(HandleSampleIDChanged);
        imageNameInput.onValueChanged.AddListener(HandleImageNameChanged);
        nextSuffixInput.onValueChanged.AddListener(HandleSuffixChanged);

        currentSampleID = (sampleIDInput.placeholder as TextMeshProUGUI)?.text;
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
        
        chooseDirectoryButton.onClick.RemoveAllListeners();
        saveFileButton.onClick.RemoveAllListeners();
        sampleIDInput.onValueChanged.RemoveAllListeners();
        imageNameInput.onValueChanged.RemoveAllListeners();
        nextSuffixInput.onValueChanged.RemoveAllListeners();
    }

    public override void EnableWindow()
    {
        base.EnableWindow();
        mapViewManager.gameObject.SetActive(true);
        mapViewClickListener.gameObject.SetActive(true);
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
    }

    void HandleMeasurement() => mapViewClickListener.HandleMeasurementToggle();
    void HandleOpenFile() => chooseDirectoryUI.EnableWindow();

    void HandleSaveFile()
    {
        var texture = RenderCameraManager.Instance.RenderNewTexture();
        var newFile = new VirtualImage(Path.Combine($"{currentSampleID}", $"{currentImageName}_{currentSuffix}.png"), texture, mapViewClickListener.CurrentMeasurementValue);
        if (!FileSystemManager.Instance.TrySaveFile(currentSampleID, newFile))
            Debug.LogError("Couldn't save file, duplicate name!");
    }

    public void HandleSampleIDChanged(string sampleID)
    {
        currentSampleID = sampleID;
        if (sampleIDInput.text != sampleID)
            sampleIDInput.text = sampleID;
        UpdateExampleName(sampleID: sampleID);
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

    void UpdateExampleName(string sampleID = null, string imageName = null, string suffix = null)
    {
        sampleID ??= currentSampleID;
        imageName ??= currentImageName;
        suffix ??= currentSuffix;
        
        exampleText.text = $"{sampleID}/{imageName}_{suffix}.png";
    }

    void HandleCloseUIButton()
    {
        mapViewManager.gameObject.SetActive(false);
        mapViewClickListener.gameObject.SetActive(false);
        OnCancelAction?.Invoke();
        CloseWindow();
    }
}
