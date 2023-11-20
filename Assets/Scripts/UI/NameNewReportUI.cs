using System;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NameNewReportUI : BaseUI
{
    [SerializeField] SingleSelectFileSystemScrollView sampleSelectScrollView;
    [SerializeField] Button createButton;
    [SerializeField] Button cancelButton;

    void OnEnable()
    {
        createButton.onClick.AddListener(HandleCreateButton);
        cancelButton.onClick.AddListener(HandleCancelButton);
    }

    void OnDisable()
    {
        createButton.onClick.RemoveAllListeners();
        cancelButton.onClick.RemoveAllListeners();
    }

    public override void EnableWindow()
    {
        base.EnableWindow();
        sampleSelectScrollView.ClearView();
        sampleSelectScrollView.AddItemsToView(WaferManager.Instance.GetSamplesWithoutReports().Select(x => x.WaferName).ToArray(), null);
    }

    void HandleCreateButton()
    {
        CloseWindow();
        OnCancelActionWithMessage?.Invoke(sampleSelectScrollView.CurrentlyHighlightedItem.ItemString);
    }

    void HandleCancelButton()
    {
        CloseWindow();
        OnCancelAction?.Invoke();
    }
}
