using System;
using UnityEngine;
using UnityEngine.UI;

public class DesktopUI : BaseUI
{
    [SerializeField] MicroscopeUI microscopeUI;
    [SerializeField] ReportGeneratorUI reportGeneratorUI;

    [SerializeField] Button microscopeUIButton;
    [SerializeField] Button reportGeneratorButton;

    void OnEnable()
    {
        microscopeUIButton.onClick.AddListener(HandleMicroscopeUIButtonPressed);
        reportGeneratorButton.onClick.AddListener(HandleReportGeneratorButtonPressed);
        
        microscopeUI.OnCancelAction = EnableWindow;
        reportGeneratorUI.OnCancelAction = EnableWindow;
    }

    void OnDisable()
    {
        microscopeUIButton.onClick.RemoveAllListeners();
        reportGeneratorButton.onClick.RemoveAllListeners();
    }

    void HandleMicroscopeUIButtonPressed()
    {
        CloseWindow();
        microscopeUI.EnableWindow();
    }

    void HandleReportGeneratorButtonPressed()
    {
        CloseWindow();
        reportGeneratorUI.EnableWindow();
    }
}
