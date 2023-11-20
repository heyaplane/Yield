using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DesktopUI : BaseUI
{
    [SerializeField] MicroscopeUI microscopeUI;
    [SerializeField] ChooseReportUI chooseReportUI;
    [SerializeField] ChatUI chatUI;

    [SerializeField] Button microscopeUIButton;
    [SerializeField] Button reportGeneratorButton;
    [SerializeField] Button messagesButton;

    [SerializeField] TextMeshProUGUI currentTimeText;

    void OnEnable()
    {
        microscopeUIButton.onClick.AddListener(HandleMicroscopeUIButton);
        reportGeneratorButton.onClick.AddListener(HandleReportGeneratorButton);
        messagesButton.onClick.AddListener(HandleMessagesButton);
        
        microscopeUI.OnCancelAction = EnableWindow;
        chooseReportUI.OnCancelAction = EnableWindow;
        chatUI.OnCancelAction = EnableWindow;

        EventManager.OnMinutesIncrementedEvent += UpdateCurrentTimeText;
        EventManager.OnHoursIncrementedEvent += UpdateCurrentTimeText;
        EventManager.OnDaysIncrementedEvent += UpdateCurrentTimeText;
    }

    void OnDisable()
    {
        microscopeUIButton.onClick.RemoveAllListeners();
        reportGeneratorButton.onClick.RemoveAllListeners();
        messagesButton.onClick.RemoveAllListeners();
        
        EventManager.OnMinutesIncrementedEvent -= UpdateCurrentTimeText;
        EventManager.OnHoursIncrementedEvent -= UpdateCurrentTimeText;
        EventManager.OnDaysIncrementedEvent -= UpdateCurrentTimeText;
    }

    void Start()
    {
        UpdateCurrentTimeText(0);
    }

    void HandleMicroscopeUIButton()
    {
        CloseWindow();
        microscopeUI.EnableWindow();
    }

    void HandleReportGeneratorButton()
    {
        chooseReportUI.EnableWindow();
    }

    void HandleMessagesButton()
    {
        CloseWindow();
        chatUI.EnableWindow();
    }

    void UpdateCurrentTimeText(int changedValue)
    {
        currentTimeText.text = $"Day {TimeSystem.Instance.CurrentDay}, {TimeSystem.Instance.CurrentHour:00}:{TimeSystem.Instance.CurrentMinute:00}";
    }
}
