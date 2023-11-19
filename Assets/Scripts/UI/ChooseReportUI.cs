using System;
using UnityEngine;
using UnityEngine.UI;

public class ChooseReportUI : BaseUI
{
    MessageData currentMessage;

    [SerializeField] Button selectButton;
    [SerializeField] Button cancelButton;

    [SerializeField] SingleSelectFileSystemScrollView reportScrollView;
    
    void OnEnable()
    {
        EventManager.OnReplyButtonClickedEvent += HandleReplyButtonClicked;
        selectButton.onClick.AddListener(HandleSelectButton);
        cancelButton.onClick.AddListener(HandleCancelButton);
    }

    void OnDisable()
    {
        EventManager.OnReplyButtonClickedEvent -= HandleReplyButtonClicked;
        selectButton.onClick.RemoveAllListeners();
        cancelButton.onClick.RemoveAllListeners();
    }

    public override void EnableWindow()
    {
        base.EnableWindow();
        currentMessage = null;
        PopulateReportScrollView();
    }

    void PopulateReportScrollView()
    {
        reportScrollView.ClearView();
        var reportNames = FileSystemManager.Instance.FindDirectoryInRoot("Reports").DirectoryFileNames;
        reportScrollView.AddItemsToView(reportNames, null);
    }

    void HandleReplyButtonClicked(MessageData messageData)
    {
        EnableWindow();
        currentMessage = messageData;
    }

    void HandleSelectButton()
    {
        if (reportScrollView.CurrentlyHighlightedItem == null)
        {
            HandleCancelButton();
            return;
        }
        
        var reportFile = FileSystemManager.Instance.FindDirectoryInRoot("Reports").FindFile(reportScrollView.CurrentlyHighlightedItem.ItemString) as VirtualReport;
        if (reportFile == null)
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
}
