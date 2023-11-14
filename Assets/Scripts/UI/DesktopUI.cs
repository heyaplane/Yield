using System;
using UnityEngine;
using UnityEngine.UI;

public class DesktopUI : BaseUI
{
    [SerializeField] MicroscopeUI microscopeUI;

    [SerializeField] Button microscopeUIButton;

    void OnEnable()
    {
        microscopeUIButton.onClick.AddListener(HandleMicroscopeUIButtonPressed);
        
        microscopeUI.OnCancelAction = EnableWindow;
    }

    void HandleMicroscopeUIButtonPressed()
    {
        CloseWindow();
        microscopeUI.EnableWindow();
    }
}
