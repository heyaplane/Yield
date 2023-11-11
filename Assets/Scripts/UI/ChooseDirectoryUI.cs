﻿using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ChooseDirectoryUI : BaseUI
{
    [SerializeField] FileSystemScrollView sampleIDScrollView;
    [SerializeField] FileSystemScrollView fileNameScrollView;

    [SerializeField] Button selectSampleIDButton;
    [SerializeField] Button cancelButton;

    [SerializeField] MicroscopeUI microscopeUI;

    [SerializeField] TextMeshProUGUI sizeText;
    [SerializeField] TextMeshProUGUI dateText;
    [SerializeField] TextMeshProUGUI typeText;
    [SerializeField] RawImage previewImage;

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
        
        var highlightedDirectory = FileSystemManager.Instance.FindDirectoryInRoot(highlight.ItemString);
        if (highlightedDirectory == null) return;
        
        fileNameScrollView.PopulateView(highlightedDirectory.DirectoryFileNames, UpdateFileDataUI);
    }

    void HandleSelectSampleIDButton()
    {
        microscopeUI.HandleSampleIDChanged(sampleIDScrollView.CurrentlyHighlightedItem.ItemString);
        HandleCancelButton();
    }

    void HandleCancelButton()
    {
        CloseWindow();
        OnCancelAction?.Invoke();
    }

    void UpdateFileDataUI(HighlightOnClick highlight)
    {
        var highlightedDirectory = FileSystemManager.Instance.FindDirectoryInRoot(sampleIDScrollView.CurrentlyHighlightedItem.ItemString);
        if (highlightedDirectory == null) return;

        var highlightedFile = highlightedDirectory.FindFile(highlight.ItemString);
        if (highlightedFile == null)
        {
            Debug.LogError("Couldn't find highlighted file!");
            return;
        }

        sizeText.text = $"Size: {highlightedFile.FileSize}";
        dateText.text = $"Date: {highlightedFile.CreationDateTime.ToShortDateString()}";
        typeText.text = $"Type: {highlightedFile.GetType()}";
        previewImage.texture = highlightedFile.Image;
    }
}