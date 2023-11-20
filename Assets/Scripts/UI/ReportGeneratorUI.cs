using System;
using System.Linq;
using ScottPlot.Statistics;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

//TODO: Add ability to view and modify previous reports, add distributions to plots for comparison

public class ReportGeneratorUI : BaseUI
{
    [SerializeField] ChooseFilesUI chooseFilesUI;
    [SerializeField] ReportPlotUI reportPlotUI;

    [SerializeField] RectTransform reportBorders;
    [SerializeField] CanvasScaler canvasScaler;
    
    [SerializeField] MultipleSelectFileSystemScrollView fileScrollView;
    
    [SerializeField] Button browseFilesButton;
    [SerializeField] Button closeUIButton;
    [SerializeField] Button saveAnalysisButton;
    [SerializeField] Button processDataButton;

    [SerializeField] TextMeshProUGUI titleText;
    [SerializeField] TextMeshProUGUI sectionNameText;
    [SerializeField] GameObject dataSummaryParent;
    [SerializeField] TextMeshProUGUI numSamplesText;
    [SerializeField] TextMeshProUGUI measuredMeanText;
    [SerializeField] TextMeshProUGUI measuredStDevText;
    [SerializeField] TextMeshProUGUI expectedMeanText;
    [SerializeField] TextMeshProUGUI expectedStDevText;

    [SerializeField] WaferSectionMapUI waferSectionMapUI;

    VirtualReport currentReport;

    void OnEnable()
    {
        EventManager.OnReportChosenEvent += HandleReportChosen;
        
        browseFilesButton.onClick.AddListener(HandleBrowseFilesButton);
        closeUIButton.onClick.AddListener(HandleCloseUIButton);
        saveAnalysisButton.onClick.AddListener(HandleSaveReportButton);
        processDataButton.onClick.AddListener(FinishGeneratingReport);

        SetupRenderCamera();
    }

    void OnDisable()
    {
        EventManager.OnReportChosenEvent -= HandleReportChosen;
        
        browseFilesButton.onClick.RemoveAllListeners();
        closeUIButton.onClick.RemoveAllListeners();
        saveAnalysisButton.onClick.RemoveAllListeners();
        processDataButton.onClick.RemoveAllListeners();
    }

    void HandleReportChosen(VirtualReport virtualReport, MessageData messageData)
    {
        if (messageData != null || virtualReport == null) return;
        currentReport = virtualReport;
        EnableWindow();
    }

    public override void EnableWindow()
    {
        base.EnableWindow();
        
        UpdateWaferSectionMap();
    }

    void UpdateWaferSectionMap() => waferSectionMapUI.Initialize(currentReport.WaferMap);

    void HandleBrowseFilesButton()
    {
        chooseFilesUI.EnableWindow();
    }

    void HandleCloseUIButton()
    {
        OnCancelAction?.Invoke();
        CloseWindow();
    }

    public void HandleFilesSelected(string[] currentlyHighlightedFileNames)
    {
        fileScrollView.AddItemsToView(currentlyHighlightedFileNames, null);
    }

    public void UpdateTitleText(string newTitle) => titleText.text = newTitle;

    void FinishGeneratingReport()
    {
        var files = FileSystemManager.Instance.GetFilesFromNames(fileScrollView.CurrentlyHighlightedFileNames);
        var measurements = files.OfType<VirtualImage>().Select(x => (double) x.MeasurementValue).ToArray();
        float mean = (float) Descriptive.Mean(measurements);
        float stDev = (float) Descriptive.StDev(measurements, mean);

        numSamplesText.text = $"# samples: {measurements.Length}";
        measuredMeanText.text = $"Mean: {mean:F2} µm";
        measuredStDevText.text = $"St. Dev: {stDev:F2} µm";
        
        reportPlotUI.AddHistogramToPlot(measurements, 0.75f);

        //currentReport = new VirtualReport(titleText.text, new ReportEntry(measurements, mean, stDev));
    }

    void HandleSaveReportButton()
    {
        FileSystemManager.Instance.TrySaveFile("Reports", currentReport);
    }

    void SetupRenderCamera()
    {
        Vector2 renderTextureScale, sizeInPixels;
        sizeInPixels = reportBorders.rect.size;
        float orthographicSize = (sizeInPixels.y / canvasScaler.referenceResolution.y) * Camera.main.orthographicSize * 2;
        
        renderTextureScale.x = sizeInPixels.x / canvasScaler.referenceResolution.x * Screen.width;
        renderTextureScale.y = sizeInPixels.y / canvasScaler.referenceResolution.y * Screen.height;
        RenderCameraManager.Instance.SetCameraAndTextureBounds(new Bounds(reportBorders.position, renderTextureScale), orthographicSize);
    }
}

[Serializable]
public class ReportEntry
{
    public string WaferName;
    public string SectionName;
    public string FeatureName;
    public double[] Measurements { get; } 
    public float Mean { get; }
    public float StDev { get; }

    public ReportEntry(string waferName, string sectionName, string featureName, double[] measurements, float mean, float stDev)
    {
        WaferName = waferName;
        SectionName = sectionName;
        FeatureName = featureName;
        Measurements = measurements;
        Mean = mean;
        StDev = stDev;
    }
}
