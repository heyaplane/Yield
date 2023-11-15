using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NameNewReportUI : BaseUI
{
    [SerializeField] ReportGeneratorUI reportGeneratorUI;
    [SerializeField] TMP_InputField inputField;
    [SerializeField] Button createButton;

    void OnEnable()
    {
        createButton.onClick.AddListener(HandleCreateButton);
    }

    void OnDisable()
    {
        createButton.onClick.RemoveAllListeners();
    }

    void HandleCreateButton()
    {
        reportGeneratorUI.UpdateTitleText(inputField.text);
        OnCancelAction?.Invoke();
        CloseWindow();
    }
}
