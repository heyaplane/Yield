using System;
using UnityEngine;
using UnityEngine.UI;

public class ChooseDirectoryUI : BaseUI
{
    [SerializeField] FileSystemScrollView sampleIDScrollView;
    [SerializeField] FileSystemScrollView fileNameScrollView;

    [SerializeField] Button selectSampleIDButton;
    [SerializeField] Button cancelButton;

    [SerializeField] MicroscopeUI microscopeUI;

    void OnEnable()
    {
        selectSampleIDButton.onClick.AddListener(HandleSelectSampleIDButton);
        cancelButton.onClick.AddListener(HandleCancelButton);
    }

    void OnDisable()
    {
        selectSampleIDButton.onClick.RemoveAllListeners();
        cancelButton.onClick.RemoveAllListeners();
    }

    public override void EnableWindow()
    {
        base.EnableWindow();
        fileNameScrollView.ClearView();
        sampleIDScrollView.ClearView();
        sampleIDScrollView.PopulateView(FileSystemManager.Instance.RootDirectory.DirectoryFileNames, PopulateFileNames);
    }

    void PopulateFileNames(HighlightOnClick highlight)
    {
        fileNameScrollView.ClearView();
        
        var highlightedDirectory = FileSystemManager.Instance.FindDirectoryInRoot(highlight.Text.text);
        if (highlightedDirectory == null) return;
        
        fileNameScrollView.PopulateView(highlightedDirectory.DirectoryFileNames, null);
    }

    void HandleSelectSampleIDButton()
    {
        microscopeUI.HandleSampleIDChanged(sampleIDScrollView.CurrentlyHighlightedItem.Text.text);
        HandleCancelButton();
    }

    void HandleCancelButton()
    {
        CloseWindow();
        OnCancelAction?.Invoke();
    }
}
