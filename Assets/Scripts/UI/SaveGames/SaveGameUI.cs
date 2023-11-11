using System;
using UnityEngine;
using UnityEngine.UI;

public class SaveGameUI : BaseUI
{
    [SerializeField] Button createNewButton;
    [SerializeField] Button playButton;
    [SerializeField] Button cancelButton;
    [SerializeField] BaseScrollView saveGameScrollView;

    [SerializeField] BaseUI enterGameNameUI;

    string saveGameName;
    void SetSelectedSaveGameName(string gameName)
    {
        saveGameName = gameName;
        saveGameScrollView.MarkItemAsSelected(gameName);
    }

    void OnEnable()
    {
        createNewButton.onClick.AddListener(HandleNewGameButton);
        playButton.onClick.AddListener(HandlePlayButton);
        cancelButton.onClick.AddListener(HandleCancelButton);
        saveGameScrollView.OnScrollViewItemClickedEvent += SetSelectedSaveGameName;
        enterGameNameUI.OnCancelActionWithMessage = CreateNewGame;
    }

    void OnDisable()
    {
        createNewButton.onClick.RemoveAllListeners();
        playButton.onClick.RemoveAllListeners();
        cancelButton.onClick.RemoveAllListeners();
        saveGameScrollView.OnScrollViewItemClickedEvent -= SetSelectedSaveGameName;
    }

    void HandleNewGameButton()
    {
        enterGameNameUI.EnableWindow();
    }

    void HandlePlayButton()
    {
        SaveManager.Instance.SaveGameInitiated(saveGameName);
    }

    void HandleCancelButton()
    {
        CloseWindow();
        OnCancelAction?.Invoke();
    }

    void CreateNewGame(string gameName)
    {
        SaveManager.Instance.CreateNewGame(gameName);
        saveGameScrollView.AddItemToScrollView(gameName);
        SetSelectedSaveGameName(gameName);
    }
}
