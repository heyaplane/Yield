using System;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuUI : BaseUI
{
    [SerializeField] TextMeshProUGUI playerNameText;

    [SerializeField] Button PlayButton;
    [SerializeField] Button optionsButton;
    [SerializeField] Button quitButton;

    [SerializeField] BaseUI optionsMenu;
    [SerializeField] BaseUI saveGameMenu;

    void OnEnable()
    {
        PlayButton.onClick.AddListener(HandlePlayButton);
        optionsButton.onClick.AddListener(HandleOptionsButton);
        quitButton.onClick.AddListener(HandleQuitButton);

        optionsMenu.OnCancelAction = EnableWindow;
        saveGameMenu.OnCancelAction = EnableWindow;
        EnableWindow();
    }

    void OnDisable()
    {
        PlayButton.onClick.RemoveAllListeners();
        optionsButton.onClick.RemoveAllListeners();
        quitButton.onClick.RemoveAllListeners();
        
        CloseWindow();
    }

    void Start()
    {
        playerNameText.text = $"Player: {SaveManager.Instance.CurrentPlayerProfile}";
    }

    void HandleOptionsButton()
    {
        CloseWindow();
        optionsMenu.EnableWindow();
    }

    void HandlePlayButton()
    {
        CloseWindow();
        saveGameMenu.EnableWindow();
    }

    void HandleQuitButton()
    {
#if UNITY_EDITOR
        EditorApplication.isPlaying = false;
#endif
        Application.Quit();
    }
}
