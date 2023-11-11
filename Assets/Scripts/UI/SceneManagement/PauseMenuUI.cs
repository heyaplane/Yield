using System;
using UnityEngine;
using UnityEngine.UI;

public class PauseMenuUI : BaseUI
{
    [SerializeField] Button resumeButton;
    [SerializeField] Button quitButton;

    [SerializeField] SaveDontSaveUI saveDontSaveUI;

    void OnEnable()
    {
        resumeButton.onClick.AddListener(() => EventManager.OnUIToggleRequested(default));
        quitButton.onClick.AddListener(HandleQuitButton);

        saveDontSaveUI.OnCancelAction = EnableWindow;
        EventManager.OnPauseMenuTriggeredEvent += EnableWindow;
    }

    void OnDisable()
    {
        resumeButton.onClick.RemoveAllListeners();
        quitButton.onClick.RemoveAllListeners();

        EventManager.OnPauseMenuTriggeredEvent -= EnableWindow;
    }

    void HandleQuitButton()
    {
        CloseWindow();
        saveDontSaveUI.EnableWindow();
    }
}
