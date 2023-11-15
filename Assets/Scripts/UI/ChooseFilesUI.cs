using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ChooseFilesUI : BaseUI
{
    [SerializeField] SingleSelectFileSystemScrollView sampleIDScrollView;
    [SerializeField] MultipleSelectFileSystemScrollView fileNameScrollView;

    [SerializeField] Button selectFilesButton;
    [SerializeField] Button cancelButton;

    [SerializeField] ReportGeneratorUI reportGeneratorUI;

    [SerializeField] TextMeshProUGUI sizeText;
    [SerializeField] TextMeshProUGUI dateText;
    [SerializeField] TextMeshProUGUI typeText;
    [SerializeField] RawImage previewImage;

    void OnEnable()
    {
        selectFilesButton.onClick.AddListener(HandleSelectFilesButton);
        cancelButton.onClick.AddListener(HandleCancelButton);
        
        previewImage.gameObject.SetActive(false);
    }

    void OnDisable()
    {
        selectFilesButton.onClick.RemoveAllListeners();
        cancelButton.onClick.RemoveAllListeners();
    }

    public override void EnableWindow()
    {
        base.EnableWindow();
        fileNameScrollView.ClearView();
        sampleIDScrollView.ClearView();
        sampleIDScrollView.AddItemsToView(FileSystemManager.Instance.RootDirectory.DirectoryFileNames, PopulateFileNames);
    }

    void PopulateFileNames(HighlightOnClick highlight)
    {
        fileNameScrollView.ClearView();
        
        var highlightedDirectory = FileSystemManager.Instance.FindDirectoryInRoot(highlight.ItemString);
        if (highlightedDirectory == null) return;
        
        fileNameScrollView.AddItemsToView(highlightedDirectory.DirectoryFileNames, UpdateFileDataUI);
    }

    void HandleSelectFilesButton()
    {
        reportGeneratorUI.HandleFilesSelected(fileNameScrollView.CurrentlyHighlightedFileNames);
        HandleCancelButton();
    }

    void HandleCancelButton()
    {
        EventManager.OnUIToggleRequested(default);
        OnCancelAction?.Invoke();
    }

    void UpdateFileDataUI(HighlightOnClick highlight)
    {
        var highlightedDirectory = FileSystemManager.Instance.FindDirectoryInRoot(sampleIDScrollView.CurrentlyHighlightedItem.ItemString);
        if (highlightedDirectory == null) return;

        var highlightedFileNames = fileNameScrollView.CurrentlyHighlightedFileNames;
        if (highlightedFileNames.Length == 0 || (highlightedFileNames.Length == 1 && !Input.GetKey(KeyCode.LeftShift) && !Input.GetKey(KeyCode.RightShift)))
        {
            var highlightedFile = highlightedDirectory.FindFile(highlight.ItemString);
            if (highlightedFile == null)
            {
                Debug.LogError("Couldn't find highlighted file!");
                return;
            }

            sizeText.text = $"Size: {highlightedFile.FileSize}";
            dateText.text = $"Date: {highlightedFile.CreationDateTime.ToShortDateString()}";
            typeText.text = $"Type: {highlightedFile.GetType()}";
            previewImage.gameObject.SetActive(false);

            void OnTextureFound(Texture2D texture)
            {
                previewImage.gameObject.SetActive(true);
                previewImage.texture = texture;
            }

            if (highlightedFile is IGeneratePreview previewGenerator)
                previewGenerator.GetPreviewImage(OnTextureFound);
        }

        else
        {
            sizeText.text = $"Size: --";
            dateText.text = $"Date: --";
            typeText.text = $"Type: --";
            previewImage.gameObject.SetActive(false);
        }
    }
}
