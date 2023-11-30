using System;
using System.Collections.Generic;
using System.Linq;
using ScottPlot.Statistics;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

//TODO: Add ability to view and modify previous reports, add distributions to plots for comparison

public class ReportEditorUI : BaseUI
{
    [SerializeField] ChooseFilesUI chooseFilesUI;
    [SerializeField] ReportPlotUI reportPlotUI;

    [SerializeField] RectTransform reportBorders;
    [SerializeField] CanvasScaler canvasScaler;
    
    [SerializeField] MultipleSelectFileSystemScrollView fileScrollView;
    
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

    [SerializeField] Toggle dataSummaryToggle;

    [SerializeField] TMP_Dropdown featureNameDropdown;
    [SerializeField] TMP_Dropdown analysisDropdown;
    [SerializeField] TMP_Dropdown processRecommendationDropdown;

    [SerializeField] WaferSectionMapUI waferSectionMapUI;

    VirtualReport currentReport;
    string currentWaferFeatureOption => featureNameDropdown.options[featureNameDropdown.value].text;
    WaferSection currentSelectedWaferSection;

    void OnEnable()
    {
        EventManager.OnReportChosenEvent += HandleReportChosen;
        
        closeUIButton.onClick.AddListener(HandleCloseUIButton);
        saveAnalysisButton.onClick.AddListener(HandleSaveAnalysisButton);
        processDataButton.onClick.AddListener(HandleProcessDataButton);
        
        dataSummaryToggle.onValueChanged.AddListener(HandleDataSummaryToggled);
        featureNameDropdown.onValueChanged.AddListener(HandleFeatureNameChanged);

        SetupRenderCamera();
        
        var options = new List<TMP_Dropdown.OptionData>
        {
            new TMP_Dropdown.OptionData("Pass"),
            new TMP_Dropdown.OptionData("Fail")
        };
        analysisDropdown.options = options;
        
        processRecommendationDropdown.ClearOptions();
        List<string> processRecommendationOptions = new List<string>(Enum.GetNames(typeof(ErrorType)));
        processRecommendationDropdown.AddOptions(processRecommendationOptions);
        processRecommendationDropdown.onValueChanged.AddListener(HandleProcessRecommendationChanged);
    }

    void OnDisable()
    {
        EventManager.OnReportChosenEvent -= HandleReportChosen;
        
        closeUIButton.onClick.RemoveAllListeners();
        saveAnalysisButton.onClick.RemoveAllListeners();
        processDataButton.onClick.RemoveAllListeners();
        
        dataSummaryToggle.onValueChanged.RemoveAllListeners();
        featureNameDropdown.onValueChanged.RemoveAllListeners();
        processRecommendationDropdown.onValueChanged.RemoveAllListeners();
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
        
        var waferMap = currentReport.WaferMap;
        var options = waferMap.WaferFeatures.Select(x => new TMP_Dropdown.OptionData(x.FeatureName)).ToList();
        featureNameDropdown.options = options;
        
        UpdateWaferSectionMap();

        if (currentReport != null)
        {
            processRecommendationDropdown.value = (int) currentReport.ProcessRecommendation;
            UpdateTitleText($"{currentReport.WaferName} Report");
        }
    }

    void HandleFeatureNameChanged(int featureNameIndex)
    {
        UpdateWaferSectionMap();
        reportPlotUI.ClearPlot();
        currentSelectedWaferSection = null;
    }

    void UpdateWaferSectionMap() => waferSectionMapUI.Initialize(currentReport, currentWaferFeatureOption, HandleWaferSectionSelected);

    void HandleCloseUIButton()
    {
        OnCancelAction?.Invoke();
        CloseWindow();
    }

    public void HandleFilesSelected(string[] currentlyHighlightedFileNames)
    {
        fileScrollView.AddItemsToView(currentlyHighlightedFileNames, null);
    }

    void HandleWaferSectionSelected(WaferSection section)
    {
        sectionNameText.text = $"Section: {section.SectionLocationAsString}";
        currentSelectedWaferSection = section;
        
        fileScrollView.ClearView();
        reportPlotUI.ClearPlot();
        
        var imageFileNames = FileSystemManager.Instance.FindDirectoryInRoot(currentReport.WaferName)?.FindFile<VirtualDirectory>(section.SectionLocationAsString)?.DirectoryFileNames;
        if (imageFileNames != null)
            fileScrollView.AddItemsToView(imageFileNames, null);
        
        var sectionDataList = currentReport.WaferMap.GetSectionDataFromLocation(section.SectionIndices);
        var currentSectionData = sectionDataList.FirstOrDefault(x => x.Feature.FeatureName == currentWaferFeatureOption);
        AddExpectedDistributionToPlot(currentSectionData);
        
        if (currentReport.TryGetReportEntry(currentSelectedWaferSection.SectionLocationAsString, currentWaferFeatureOption, out var reportEntry) && reportEntry.Measurements != null)
            UpdateMeasuredData(reportEntry);
            
    }

    public void UpdateTitleText(string newTitle) => titleText.text = newTitle;

    void AddExpectedDistributionToPlot(SectionData sectionData)
    {
        expectedMeanText.text = $"{sectionData.Mean:F2} {sectionData.Feature.Units}";
        expectedStDevText.text = $"{sectionData.StDev:F2} {sectionData.Feature.Units}";
        
        if (sectionData.Mean != 0)
            reportPlotUI.AddGaussianToPlot(sectionData.Mean, sectionData.StDev, stDevRange:5);
    }

    void HandleProcessDataButton()
    {
        var files = FileSystemManager.Instance.GetFilesFromNames(fileScrollView.CurrentlyHighlightedFileNames);
        var measurements = files.OfType<VirtualImage>().Select(x => (double) x.MeasurementValue).ToArray();
        
        float mean = (float) Descriptive.Mean(measurements);
        float stDev = (float) Descriptive.StDev(measurements, mean);
        
        if (currentReport.TryGetReportEntry(currentSelectedWaferSection.SectionLocationAsString, currentWaferFeatureOption, out var reportEntry))
        {
            reportEntry.Measurements = measurements;
            reportEntry.Mean = mean;
            reportEntry.StDev = stDev;
        }

        else
        {
            reportEntry = new ReportEntry(currentReport.WaferName, currentSelectedWaferSection.SectionLocationAsString, currentWaferFeatureOption, ReportEntryState.DataExist);
            currentReport.AddReportEntry(currentSelectedWaferSection.SectionLocationAsString, reportEntry);
        }
        
        UpdateMeasuredData(reportEntry);
    }

    void UpdateMeasuredData(ReportEntry reportEntry)
    {
        reportPlotUI.AddKDEToPlot(reportEntry.Measurements, reportEntry.StDev);

        numSamplesText.text = $"{reportEntry.Measurements.Length}";
        measuredMeanText.text = $"{reportEntry.Mean:F2} µm";
        measuredStDevText.text = $"{reportEntry.StDev:F2} µm";
    }

    void HandleSaveAnalysisButton()
    {
        if (!currentReport.TryGetReportEntry(currentSelectedWaferSection.SectionLocationAsString, currentWaferFeatureOption, out var reportEntry)) return;

        switch (analysisDropdown.value)
        {
            case 0:
                reportEntry.State = ReportEntryState.Pass;
                break;
            case 1:
                reportEntry.State = ReportEntryState.Fail;
                break;
            default:
                Debug.LogError("Couldn't identify analysis option.");
                break;
        }
        
        UpdateWaferSectionMap();
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

    void HandleDataSummaryToggled(bool toggleOn) => dataSummaryParent.SetActive(toggleOn);

    void HandleProcessRecommendationChanged(int newValue) => currentReport.ProcessRecommendation = (ErrorType) newValue;
}
