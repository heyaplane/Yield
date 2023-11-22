using System;
using UnityEngine;
using UnityEngine.UI;

public class ChooseReportUI : BaseUI
{
    MessageData currentMessage;

    [SerializeField] Button selectButton;
    [SerializeField] Button cancelButton;
    [SerializeField] Button createNewReportButton;

    [SerializeField] SingleSelectFileSystemScrollView reportScrollView;

    [SerializeField] NameNewReportUI nameNewReportUI;
    [SerializeField] ReportEditorUI reportEditorUI;
    
    void OnEnable()
    {
        EventManager.OnReplyButtonClickedEvent += HandleReplyButtonClicked;
        selectButton.onClick.AddListener(HandleSelectButton);
        cancelButton.onClick.AddListener(HandleCancelButton);
        createNewReportButton.onClick.AddListener(HandleCreateNewReportButton);
    }

    void OnDisable()
    {
        EventManager.OnReplyButtonClickedEvent -= HandleReplyButtonClicked;
        selectButton.onClick.RemoveAllListeners();
        cancelButton.onClick.RemoveAllListeners();
        createNewReportButton.onClick.RemoveAllListeners();
    }

    public override void EnableWindow()
    {
        base.EnableWindow();
        currentMessage = null;
        PopulateReportScrollView();
        
        createNewReportButton.gameObject.SetActive(true);
        nameNewReportUI.OnCancelActionWithMessage = FinishGeneratingNewReport;
        nameNewReportUI.OnCancelAction = EnableWindow;
    }

    void PopulateReportScrollView()
    {
        reportScrollView.ClearView();
        var reportNames = FileSystemManager.Instance.FindDirectoryInRoot("Reports")?.DirectoryFileNames;
        reportScrollView.AddItemsToView(reportNames, null);
    }

    void HandleReplyButtonClicked(MessageData messageData)
    {
        EnableWindow();
        
        currentMessage = messageData;
        createNewReportButton.gameObject.SetActive(false);
    }

    void HandleSelectButton()
    {
        if (reportScrollView.CurrentlyHighlightedItem == null) return;

        if (FileSystemManager.Instance.FindDirectoryInRoot("Reports").FindFile(reportScrollView.CurrentlyHighlightedItem.ItemString) is not VirtualReport reportFile)
        {
            Debug.LogError("Highlighted report is not a VirtualReport!");
            return;
        }

        EventManager.OnReportChosen(reportFile, currentMessage);
        CloseWindow();
    }

    void HandleCancelButton()
    {
        EventManager.OnReportChosen(null, currentMessage);
        CloseWindow();
    }

    void HandleCreateNewReportButton()
    {
        CloseWindow();
        nameNewReportUI.EnableWindow();
    }

    void FinishGeneratingNewReport(string waferName)
    {
        var newReport = new VirtualReport(waferName, WaferManager.Instance.GetWaferDataFromName(waferName));
        EventManager.OnReportChosen(newReport, null);
    }
}
