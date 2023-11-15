using System;
using System.Linq;
using ScottPlot.Statistics;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ReportGeneratorUI : BaseUI
{
    [SerializeField] ChooseFilesUI chooseFilesUI;
    [SerializeField] NameNewReportUI nameNewReportUI;
    [SerializeField] ReportPlotUI reportPlotUI;

    [SerializeField] RectTransform reportBorders;
    [SerializeField] CanvasScaler canvasScaler;
    
    [SerializeField] SingleSelectFileSystemScrollView fileScrollView;
    
    [SerializeField] Button browseFilesButton;
    [SerializeField] Button closeUIButton;
    [SerializeField] Button generateReportButton;
    [SerializeField] Button saveReportButton;

    [SerializeField] TextMeshProUGUI titleText;
    [SerializeField] TextMeshProUGUI numSamplesText;
    [SerializeField] TextMeshProUGUI meanText;
    [SerializeField] TextMeshProUGUI stDevText;

    void OnEnable()
    {
        browseFilesButton.onClick.AddListener(HandleBrowseFilesButton);
        closeUIButton.onClick.AddListener(HandleCloseUIButton);
        generateReportButton.onClick.AddListener(HandleGenerateReportButton);
        saveReportButton.onClick.AddListener(HandleSaveReportButton);

        nameNewReportUI.OnCancelAction = FinishGeneratingReport;
        SetupRenderCamera();
    }

    void OnDisable()
    {
        browseFilesButton.onClick.RemoveAllListeners();
        closeUIButton.onClick.RemoveAllListeners();
        generateReportButton.onClick.RemoveAllListeners();
        saveReportButton.onClick.RemoveAllListeners();
    }
    
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

    void HandleGenerateReportButton() => nameNewReportUI.EnableWindow();

    void FinishGeneratingReport()
    {
        var files = FileSystemManager.Instance.GetFilesFromNames(fileScrollView.GetAllItemNames());
        var measurements = files.OfType<VirtualImage>().Select(x => (double) x.MeasurementValue).ToArray();
        float mean = (float) Descriptive.Mean(measurements);
        float stDev = (float) Descriptive.StDev(measurements, mean);

        numSamplesText.text = $"# samples: {measurements.Length}";
        meanText.text = $"Mean: {mean:F2} µm";
        stDevText.text = $"St. Dev: {stDev:F2} µm";
        
        reportPlotUI.AddHistogramToPlot(measurements, 0.75f);
    }

    void HandleSaveReportButton()
    {
        
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
